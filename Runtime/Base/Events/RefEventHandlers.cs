using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace CnoomFrameWork.Base.Events
{
    /// <summary>
    ///     引用类型事件处理器，处理结构体事件
    /// </summary>
    public class RefEventHandlers : TEventHandler
    {
        /// <summary>
        ///     ref 结构体事件处理器委托
        /// </summary>
        public delegate void RefEvent<T>(ref T e) where T : struct;

        /// <summary>
        ///     注册结构体事件处理器（ref 传参）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe<T>(RefEvent<T> handler, int priority = 0, bool once = false)
            where T : struct
        {
            AddHandler(typeof(T), handler, priority, once);
        }

        /// <summary>
        ///     取消注册结构体事件处理器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnSubscribe<T>(RefEvent<T> handler) where T : struct
        {
            var type = typeof(T);
            lock (WriteLock)
            {
                if (Handlers.TryGetValue(type, out var list)) 
                    list.RemoveAll(h => h.Handler == (Delegate)handler);
            }
        }

        /// <summary>
        ///     同步触发结构体事件，允许处理器修改结构体内容
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish<T>(ref T e) where T : struct
        {
            var type = typeof(T);
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
                if (h.Handler is RefEvent<T> handler)
                    handler(ref e);
                
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
        ///     添加事件过滤器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFilter<T>(Func<T, Delegate, bool> filter) where T : struct
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
                foreach (var attr in m.GetCustomAttributes<RefEventSubscriberAttribute>())
                {
                    var type = attr.EventType;
                    var handlerType = typeof(RefEvent<>).MakeGenericType(type);
                    var del = CreateDelegate(handlerType, subscriber, m);
                    AddHandler(type, del, attr.Priority, attr.Once);
                }
            }
        }
    }
}