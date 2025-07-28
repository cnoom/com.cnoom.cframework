using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CnoomFrameWork.Core.Base.Pool;
using Object = UnityEngine.Object;

namespace CnoomFrameWork.Base.Events
{
    /// <summary>
    ///     普通事件处理器，处理引用类型事件
    /// </summary>
    public class EventHandler : TEventHandler
    {
        // 事件对象池，减少GC压力
        private readonly Dictionary<Type, ObjectPool<object>> _eventPools = new();
        
        /// <summary>
        ///     注册同步普通事件处理器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe<T>(Action<T> handler, int priority = 0, bool once = false)
        {
            AddHandler(typeof(T), handler, priority, once);
        }

        /// <summary>
        ///     取消注册普通事件处理器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            lock (WriteLock)
            {
                if (Handlers.TryGetValue(type, out var list)) 
                    list.RemoveAll(h => h.Handler == (Delegate)handler);
            }
        }

        /// <summary>
        ///     同步发布事件给所有订阅者
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish<T>(T e)
        {
            Type type = typeof(T);
            List<HandlerInfo> snapshot;
            
            // 减少锁的范围，只在读取时加锁
            lock (ReadLock)
            {
                if (!Handlers.TryGetValue(type, out var list) || list.Count == 0) 
                    return;
                    
                snapshot = HandlerListPool.Get();
                snapshot.AddRange(list);
            }

            List<HandlerInfo> toRemove = null;

            // 遍历处理器并调用
            for (int i = 0; i < snapshot.Count; i++)
            {
                var h = snapshot[i];
                
                // 检查目标是否有效
                if (!h.Target.TryGetTarget(out var t) || (t is Object o && !o))
                {
                    if (toRemove == null) toRemove = HandlerListPool.Get();
                    toRemove.Add(h);
                    continue;
                }

                // 检查过滤器
                if (!ShouldInvokeHandler(e, h.Handler)) continue;
                
                // 调用处理器
                if (h.Handler is Action<T> syncHandler)
                    syncHandler(e);

                // 处理一次性事件
                if (h.Once)
                {
                    if (toRemove == null) toRemove = HandlerListPool.Get();
                    toRemove.Add(h);
                }
            }

            // 移除需要移除的处理器
            if (toRemove != null)
                TryRemoveHandle(type, toRemove);
                
            HandlerListPool.Release(snapshot);
            
            // 定期清理无效处理器
            CleanupDeadHandlers();
        }
        
        /// <summary>
        ///     创建并获取可重用的事件对象，减少GC压力
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetEventObject<T>() where T : class, new()
        {
            var type = typeof(T);
            
            lock (ReadLock)
            {
                if (!_eventPools.TryGetValue(type, out var pool))
                {
                    lock (WriteLock)
                    {
                        if (!_eventPools.TryGetValue(type, out pool))
                        {
                            pool = new ObjectPool<object>(
                                () => new T(),
                                null,
                                obj => 
                                {
                                    // 如果事件对象实现了IResetable接口，则重置它
                                    if (obj is IResetable resetable)
                                        resetable.Reset();
                                });
                            _eventPools[type] = pool;
                        }
                    }
                }
                
                return (T)pool.Get();
            }
        }
        
        /// <summary>
        ///     释放事件对象回对象池
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseEventObject<T>(T eventObj) where T : class
        {
            var type = typeof(T);
            
            lock (ReadLock)
            {
                if (_eventPools.TryGetValue(type, out var pool))
                {
                    pool.Release(eventObj);
                }
            }
        }

        /// <summary>
        ///     添加普通事件过滤器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFilter<T>(Func<T, Delegate, bool> filter)
        {
            var type = typeof(T);
            lock (WriteLock)
            {
                if (!Filters.TryGetValue(type, out var list))
                    list = Filters[type] = new List<Delegate>();
                list.Add(filter);
            }
        }

        /// <summary>
        ///     扫描对象中带有特性的方法并自动注册为事件处理器
        /// </summary>
        public override void Register(object subscriber)
        {
            var methods = GetMethodInfo(subscriber);

            foreach (var m in methods)
            {
                foreach (var attr in m.GetCustomAttributes<EventSubscriberAttribute>())
                {
                    var type = attr.EventType;
                    var delegateType = typeof(Action<>).MakeGenericType(type);
                    var del = GetOrCreateDelegate(delegateType, subscriber, m);

                    AddHandler(type, del, attr.Priority, attr.Once);
                }
            }
        }
    }
    
    /// <summary>
    ///     可重置接口，用于事件对象池中的对象重置
    /// </summary>
    public interface IResetable
    {
        void Reset();
    }
}
