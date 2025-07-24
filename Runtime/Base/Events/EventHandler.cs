using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Object = UnityEngine.Object;

namespace CnoomFrameWork.Base.Events
{
    public class EventHandler : TEventHandler
    {
        /// <summary>
        ///     注册同步普通事件处理器。
        /// </summary>
        public void Subscribe<T>(Action<T> handler, int priority = 0, bool once = false)
        {
            AddHandler(typeof(T), handler, priority, once);
        }

        /// <summary>
        ///     取消注册普通事件处理器。
        /// </summary>
        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            lock (Lock)
            {
                if (Handlers.TryGetValue(type, out var list)) list.RemoveAll(h => h.Handler == (Delegate)handler);
            }
        }


        /// <summary>
        ///     同步发布事件给所有订阅者。
        /// </summary>
        public void Publish<T>(T e)
        {
            Type type = typeof(T);
            List<HandlerInfo> snapshot;
            lock (Lock)
            {
                if (!Handlers.TryGetValue(type, out var list)) return;
                snapshot = HandlerListPool.Get();
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
                if (h.Handler is Action<T> syncHandler)
                    syncHandler(e);

                if (h.Once)
                    toRemove.Add(h);
            }

            TryRemoveHandle(type, toRemove);
            HandlerListPool.Release(snapshot);
        }

        /// <summary>
        ///     添加普通事件过滤器。每个处理器会独立判断是否继续传播。
        /// </summary>
        public void AddFilter<T>(Func<T, Delegate, bool> filter)
        {
            var type = typeof(T);
            lock (Lock)
            {
                if (!Filters.TryGetValue(type, out var list))
                    list = Filters[type] = new List<Delegate>();
                list.Add(filter);
            }
        }

        /// <summary>
        ///     扫描对象中带有特性的方法并自动注册为事件处理器。
        /// </summary>
        public override void Register(object subscriber)
        {
            var methods = GetMethodInfo(subscriber);

            foreach (var m in methods)
            {
                foreach (var attr in m.GetCustomAttributes<EventSubscriberAttribute>())
                {
                    var type = attr.EventType;
                    var del = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(type), subscriber, m);

                    AddHandler(type, del, attr.Priority, attr.Once);
                }
            }
        }
    }
}