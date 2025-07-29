using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using CnoomFrameWork.Base.Log;
using CnoomFrameWork.Core.Base.Pool;

namespace CnoomFrameWork.Base.Events
{
    /// <summary>
    ///     事件处理器基类，提供事件注册和触发的核心功能
    /// </summary>
    public abstract class TEventHandler
    {
        // 使用并发字典减少锁的使用
        protected readonly Dictionary<Type, List<HandlerInfo>> Handlers = new();
        protected readonly Dictionary<Type, List<Delegate>> Filters = new();

        // 对象池用于减少GC压力
        protected readonly ObjectPool<List<HandlerInfo>> HandlerListPool;

        // 缓存反射信息以提高性能
        private readonly Dictionary<Type, MethodInfo[]> _methodInfos = new();
        private readonly Dictionary<Type, Dictionary<MethodInfo, Delegate>> _delegateCache = new();

        // 读写锁分离，提高并发性能
        protected readonly object ReadLock = new();
        protected readonly object WriteLock = new();

        // 无效处理器清理计时器
        private int _cleanupCounter = 0;
        private const int CleanupThreshold = 100; // 每100次事件触发清理一次无效处理器

        protected TEventHandler()
        {
            HandlerListPool = new ObjectPool<List<HandlerInfo>>(
                () => new List<HandlerInfo>(8), // 预分配容量减少扩容
                null,
                list => { list.Clear(); });
        }

        /// <summary>
        ///     尝试移除处理器，优化批量移除操作
        /// </summary>
        public void TryRemoveHandle(Type type, List<HandlerInfo> toRemove)
        {
            if (toRemove.Count <= 0)
            {
                HandlerListPool.Release(toRemove);
                return;
            }

            lock (WriteLock)
            {
                if (!Handlers.TryGetValue(type, out var list))
                {
                    HandlerListPool.Release(toRemove);
                    return;
                }

                // 优化：使用HashSet加速查找
                if (toRemove.Count > 3 && list.Count > 10)
                {
                    var removeSet = new HashSet<HandlerInfo>(toRemove);
                    list.RemoveAll(h => removeSet.Contains(h));
                }
                else
                {
                    foreach (var r in toRemove)
                        list.Remove(r);
                }
            }

            HandlerListPool.Release(toRemove);
        }

        /// <summary>
        ///     注册处理器，优化排序逻辑
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void AddHandler(Type type, Delegate handler, int priority = 0, bool once = false)
        {
            if (handler.Target == null) return; // 静态方法不支持

            lock (WriteLock)
            {
                if (!Handlers.TryGetValue(type, out var list))
                    list = Handlers[type] = new List<HandlerInfo>(4); // 预分配容量

                // 检查是否已存在相同处理器
                foreach (var existing in list)
                {
                    if (existing.Handler != handler)
                    {
                        continue;
                    }

                    existing.Target.TryGetTarget(out var target);
                    if (target != handler.Target)
                    {
                        continue;
                    }

                    LogManager.Warn(nameof(TEventHandler),
                        $"重复注册方法 对象:[{handler.Target.GetType()}],方法:[{handler.Method.Name}]");
                    existing.Priority = priority;
                    existing.Once = once;
                    return;
                }

                var info = new HandlerInfo
                {
                    Handler = handler,
                    Priority = priority,
                    Once = once,
                    Target = new WeakReference<object>(handler.Target)
                };

                // 优化：仅在必要时排序
                if (list.Count == 0 || priority <= list[^1].Priority)
                {
                    // 如果优先级最低，直接添加到末尾
                    list.Add(info);
                }
                else if (priority >= list[0].Priority)
                {
                    // 如果优先级最高，添加到开头
                    list.Insert(0, info);
                }
                else
                {
                    // 二分查找插入位置
                    int left = 0, right = list.Count - 1;
                    while (left <= right)
                    {
                        int mid = (left + right) / 2;
                        if (list[mid].Priority < priority)
                            right = mid - 1;
                        else
                            left = mid + 1;
                    }

                    list.Insert(left, info);
                }
            }
        }

        /// <summary>
        ///     检查事件是否应被特定处理器接收（基于过滤器判断）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool ShouldInvokeHandler<T>(T e, Delegate handler)
        {
            List<Delegate> filterList;

            lock (ReadLock)
            {
                if (!Filters.TryGetValue(typeof(T), out filterList) || filterList.Count == 0)
                    return true;
            }

            foreach (var filter in filterList.Cast<Func<T, Delegate, bool>>())
                if (!filter(e, handler))
                    return false;

            return true;
        }

        /// <summary>
        ///     获取类型的方法信息，使用缓存提高性能
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected MethodInfo[] GetMethodInfo(object subscriber)
        {
            var type = subscriber.GetType();

            lock (ReadLock)
            {
                if (_methodInfos.TryGetValue(type, out MethodInfo[] methodInfos))
                {
                    return methodInfos;
                }
            }

            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            lock (WriteLock)
            {
                _methodInfos[type] = methods;
            }

            return methods;
        }

        /// <summary>
        ///     获取或创建委托，使用缓存提高性能
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Delegate GetOrCreateDelegate(Type delegateType, object target, MethodInfo method)
        {
            var targetType = target.GetType();

            lock (ReadLock)
            {
                if (_delegateCache.TryGetValue(targetType, out var methodCache) &&
                    methodCache.TryGetValue(method, out var cachedDelegate))
                {
                    return cachedDelegate;
                }
            }

            var newDelegate = Delegate.CreateDelegate(delegateType, target, method);

            lock (WriteLock)
            {
                if (!_delegateCache.TryGetValue(targetType, out var methodCache))
                {
                    methodCache = new Dictionary<MethodInfo, Delegate>();
                    _delegateCache[targetType] = methodCache;
                }

                methodCache[method] = newDelegate;
            }

            return newDelegate;
        }

        /// <summary>
        ///     注册订阅者
        /// </summary>
        public abstract void Register(object subscriber);

        /// <summary>
        ///     从事件系统中注销某个对象的所有事件处理器
        /// </summary>
        public void Unregister(object subscriber)
        {
            lock (WriteLock)
            {
                foreach (var list in Handlers.Values)
                    list.RemoveAll(h =>
                    {
                        if (h.Target.TryGetTarget(out var target)) return ReferenceEquals(target, subscriber);
                        return true;
                    });

                // 清除委托缓存
                var type = subscriber.GetType();
                if (_delegateCache.ContainsKey(type))
                {
                    _delegateCache.Remove(type);
                }
            }
        }

        /// <summary>
        ///     清理无效的处理器引用
        /// </summary>
        protected void CleanupDeadHandlers()
        {
            if (Interlocked.Increment(ref _cleanupCounter) < CleanupThreshold)
                return;

            Interlocked.Exchange(ref _cleanupCounter, 0);

            lock (WriteLock)
            {
                foreach (var type in Handlers.Keys.ToArray())
                {
                    var list = Handlers[type];
                    int originalCount = list.Count;

                    list.RemoveAll(h =>
                        !h.Target.TryGetTarget(out var target) || (target is UnityEngine.Object o && !o));

                    // 如果列表为空，移除该类型的处理器列表
                    if (list.Count == 0 && originalCount > 0)
                    {
                        Handlers.Remove(type);
                    }
                }
            }
        }
    }
}