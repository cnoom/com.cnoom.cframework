using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CnoomFrameWork.Base.Events
{
    public abstract class TEventHandler
    {
        protected readonly Dictionary<Type, List<HandlerInfo>> Handlers = new();
        protected readonly Dictionary<Type, List<Delegate>> Filters = new();
        protected readonly Dictionary<Type, MethodInfo[]> MethodInfos = new();
        protected readonly object Lock = new();

        /// <summary>
        ///     注册处理器。
        /// </summary>
        protected void AddHandler(Type type, Delegate handler, int priority = 0, bool once = false)
        {
            lock (Lock)
            {
                if (!Handlers.TryGetValue(type, out var list))
                    list = Handlers[type] = new List<HandlerInfo>();

                list.Add(new HandlerInfo
                {
                    Handler = handler,
                    Priority = priority,
                    Once = once,
                    Target = new WeakReference<object>(handler.Target)
                });
                list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            }
        }

        /// <summary>
        ///     添加事件过滤器。每个处理器会独立判断是否继续传播。
        /// </summary>
        public void AddFilter<T>(Func<T, Delegate, bool> filter) where T : struct
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
        ///     检查事件是否应被特定处理器接收（基于过滤器判断）。
        /// </summary>
        protected bool ShouldInvokeHandler<T>(T e, Delegate handler)
        {
            if (Filters.TryGetValue(typeof(T), out var filterList))
                foreach (var filter in filterList.Cast<Func<T, Delegate, bool>>())
                    if (!filter(e, handler))
                        return false;

            return true;
        }

        protected MethodInfo[] GetMethodInfo(object subscriber)
        {
            var type = subscriber.GetType();
            if (MethodInfos.TryGetValue(type, out MethodInfo[] methodInfos))
            {
                return methodInfos;
            }

            methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            MethodInfos.Add(type, methodInfos);
            return methodInfos;
        }

        public abstract void Register(object subscriber);

        /// <summary>
        ///     从事件系统中注销某个对象的所有事件处理器。
        /// </summary>
        public void Unregister(object subscriber)
        {
            lock (Lock)
            {
                foreach (var list in Handlers.Values)
                    list.RemoveAll(h =>
                    {
                        if (h.Target.TryGetTarget(out var target)) return ReferenceEquals(target, subscriber);
                        return true;
                    });
            }
        }
    }
}