using System;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

namespace CnoomFrameWork.Base.Events
{
    public class RefEventHandlers : TEventHandler
    {
        /// <summary>
        ///     ref 结构体事件处理器委托。
        /// </summary>
        public delegate void RefEvent<T>(ref T e) where T : struct;

        /// <summary>
        ///     注册结构体事件处理器（ref 传参）。
        /// </summary>
        public void Subscribe<T>(RefEvent<T> handler, int priority = 0, bool once = false)
            where T : struct
        {
            AddHandler(typeof(T), handler, priority, once);
        }

        /// <summary>
        ///     取消注册结构体事件处理器。
        /// </summary>
        public void UnSubscribe<T>(RefEvent<T> handler) where T : struct
        {
            var type = typeof(T);
            lock (Lock)
            {
                if (Handlers.TryGetValue(type, out var list)) list.RemoveAll(h => h.Handler == (Delegate)handler);
            }
        }

        /// <summary>
        ///     同步触发结构体事件，允许处理器修改结构体内容。
        /// </summary>
        public void Publish<T>(ref T e) where T : struct
        {
            var type = typeof(T);
            List<HandlerInfo> snapshot;
            lock (Lock)
            {
                if (!Handlers.TryGetValue(type, out var list)) return;
                snapshot = HandlerListPool.Get();
                snapshot.AddRange(list);
            }

            List<HandlerInfo> toRemove = HandlerListPool.Get();

            foreach (var h in snapshot)
            {
                if (!h.Target.TryGetTarget(out var t) || (t is Object o && !o))
                {
                    toRemove.Add(h);
                    continue;
                }

                if (!ShouldInvokeHandler(e, h.Handler)) continue;
                if (h.Handler is not RefEvent<T> handler) continue;
                handler(ref e);
                if (h.Once)
                    toRemove.Add(h);
            }

            TryRemoveHandle(type, toRemove);
            HandlerListPool.Release(snapshot);
        }

        public override void Register(object subscriber)
        {
            var methods = GetMethodInfo(subscriber);

            foreach (var m in methods)
            {
                foreach (var attr in m.GetCustomAttributes<RefEventSubscriberAttribute>())
                {
                    var type = attr.EventType;
                    var handlerType = typeof(RefEvent<>).MakeGenericType(type);
                    var del = Delegate.CreateDelegate(handlerType, subscriber, m);
                    AddHandler(type, del, attr.Priority, attr.Once);
                }
            }
        }
    }
}