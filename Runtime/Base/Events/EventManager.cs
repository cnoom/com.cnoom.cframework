using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CnoomFrameWork.Base.Log;
using CnoomFrameWork.Core;
using Object = UnityEngine.Object;

namespace CnoomFrameWork.Base.Events
{
    public class EventManager
    {
        /// <summary>
        /// ref 结构体事件处理器委托。
        /// </summary>
        public delegate void RefEventHandler<T>(ref T e) where T : struct;

        internal static App App;

        private static readonly Dictionary<Type, List<HandlerInfo>> Handlers = new();
        private static readonly Dictionary<Type, List<RefHandlerInfo>> RefHandlers = new();
        private static readonly Dictionary<Type, List<Delegate>> Filters = new();
        private static readonly Dictionary<Type, List<Delegate>> RefFilters = new();
        private static readonly object Lock = new();
        private static readonly object RefLock = new();

        /// <summary>
        /// 注册异步普通事件处理器。
        /// </summary>
        public static void Subscribe<T>(Func<T, Task> handler, int priority = 0, bool once = false)
        {
            AddHandler(typeof(T), handler, priority, once);
        }

        /// <summary>
        /// 注册同步普通事件处理器。
        /// </summary>
        public static void Subscribe<T>(Action<T> handler, int priority = 0, bool once = false)
        {
            AddHandler(typeof(T), handler, priority, once);
        }

        /// <summary>
        /// 取消注册普通事件处理器。
        /// </summary>
        public static void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            lock (Lock)
            {
                if (Handlers.TryGetValue(type, out var list))
                {
                    list.RemoveAll(h => h.Handler == (Delegate)handler);
                }
            }
        }

        private static void AddHandler(Type type, Delegate handler, int priority, bool once)
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
        /// 检查事件是否应被特定处理器接收（基于过滤器判断）。
        /// </summary>
        private static bool ShouldInvokeHandler<T>(T e, Delegate handler)
        {
            if (Filters.TryGetValue(typeof(T), out var filterList))
            {
                foreach (Func<T, Delegate, bool> filter in filterList.Cast<Func<T, Delegate, bool>>())
                {
                    if (!filter(e, handler)) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 检查结构体事件是否应被特定处理器接收（基于过滤器判断）。
        /// </summary>
        private static bool ShouldInvokeRefHandler<T>(T e, Delegate handler) where T : struct
        {
            if (RefFilters.TryGetValue(typeof(T), out var filterList))
            {
                foreach (Delegate filterObject in filterList)
                {
                    // 确保过滤器是正确的类型
                    if (filterObject is Func<T, Delegate, bool> filter)
                    {
                        if (!filter(e, handler)) return false;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"过滤器类型不匹配: {filterObject.GetType()} 不是 Func<{typeof(T)}, Delegate, bool>");
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 同步或异步发布事件给所有订阅者。
        /// </summary>
        public static void Publish<T>(T e)
        {
            List<HandlerInfo> snapshot;
            lock (Lock)
            {
                if (!Handlers.TryGetValue(typeof(T), out var list)) return;
                snapshot = new List<HandlerInfo>(list);
            }

            HashSet<HandlerInfo> toRemove = new();

            foreach (var h in snapshot)
            {
                if (!h.Target.TryGetTarget(out object t) || t is Object o && !o)
                {
                    toRemove.Add(h);
                    continue;
                }

                if (!ShouldInvokeHandler(e, h.Handler)) continue;
                try
                {
                    if (h.Handler is Action<T> syncHandler)
                        syncHandler(e);

                    if (h.Once)
                        toRemove.Add(h);
                }
                catch (System.Exception ex)
                {
                    App.Log.Log($"[EventManager] Error: {ex.Message}\n{ex.StackTrace}", ELogType.Error);
                }
            }

            if (toRemove.Count > 0)
            {
                lock (Lock)
                {
                    if (Handlers.TryGetValue(typeof(T), out var list))
                    {
                        foreach (var r in toRemove)
                            list.Remove(r);
                    }
                }
            }
        }

        /// <summary>
        /// 注册结构体事件处理器（ref 传参）。
        /// </summary>
        public static void SubscribeRef<T>(RefEventHandler<T> handler, int priority = 0, bool once = false)
            where T : struct
        {
            var type = typeof(T);
            lock (RefLock)
            {
                if (!RefHandlers.TryGetValue(type, out var list))
                    list = RefHandlers[type] = new List<RefHandlerInfo>();

                list.Add(new RefHandlerInfo
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
        /// 取消注册结构体事件处理器。
        /// </summary>
        public static void UnSubscribeRef<T>(RefEventHandler<T> handler) where T : struct
        {
            var type = typeof(T);
            lock (RefLock)
            {
                if (RefHandlers.TryGetValue(type, out var list))
                {
                    list.RemoveAll(h => h.Handler == (Delegate)handler);
                }
            }
        }

        /// <summary>
        /// 同步触发结构体事件，允许处理器修改结构体内容。
        /// </summary>
        public static void RefPublish<T>(ref T e) where T : struct
        {
            var type = typeof(T);
            List<RefHandlerInfo> snapshot;
            lock (RefLock)
            {
                if (!RefHandlers.TryGetValue(type, out var list)) return;
                snapshot = new List<RefHandlerInfo>(list);
            }

            HashSet<RefHandlerInfo> toRemove = new();

            foreach (var h in snapshot)
            {
                if (!h.Target.TryGetTarget(out object t) || t is Object o && !o)
                {
                    toRemove.Add(h);
                    continue;
                }

                if (!ShouldInvokeRefHandler(e, h.Handler)) continue;
                try
                {
                    if (h.Handler is RefEventHandler<T> handler)
                    {
                        handler(ref e);
                        if (h.Once)
                            toRemove.Add(h);
                    }
                }
                catch (System.Exception ex)
                {
                    App.Log.Log($"[EventManager] Error: {ex.Message}\n{ex.StackTrace}", ELogType.Error);
                }
            }

            if (toRemove.Count > 0)
            {
                lock (RefLock)
                {
                    if (RefHandlers.TryGetValue(type, out var list))
                    {
                        foreach (RefHandlerInfo r in toRemove)
                            list.Remove(r);
                    }
                }
            }
        }

        /// <summary>
        /// 添加普通事件过滤器。每个处理器会独立判断是否继续传播。
        /// </summary>
        public static void AddFilter<T>(Func<T, Delegate, bool> filter)
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
        /// 添加结构体事件过滤器。每个处理器会独立判断是否继续传播。
        /// </summary>
        public static void AddRefFilter<T>(Func<T, Delegate, bool> filter) where T : struct
        {
            var type = typeof(T);
            lock (RefLock)
            {
                if (!RefFilters.TryGetValue(type, out var list))
                    list = RefFilters[type] = new List<Delegate>();
                list.Add(filter);
            }
        }

        /// <summary>
        /// 扫描对象中带有特性的方法并自动注册为事件处理器。
        /// </summary>
        public static void Register(object subscriber)
        {
            var methods = subscriber.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var m in methods)
            {
                bool canSkip = false;
                foreach (var attr in m.GetCustomAttributes<EventSubscriberAttribute>())
                {
                    var type = attr.EventType;
                    try
                    {
                        Delegate del = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(type), subscriber, m);

                        AddHandler(type, del, attr.Priority, attr.Once);
                        canSkip = true;
                    }
                    catch (System.Exception ex)
                    {
                        App.Log.Log(
                            $"[EventManager] Register Failed to bind ref method[{m.Name}]: {ex.Message}\n{ex.StackTrace}",
                            ELogType.Error);
                    }
                }

                if (canSkip) continue;

                foreach (var attr in m.GetCustomAttributes<RefEventSubscriberAttribute>())
                {
                    var type = attr.EventType;
                    try
                    {
                        var handlerType = typeof(RefEventHandler<>).MakeGenericType(type);
                        var del = Delegate.CreateDelegate(handlerType, subscriber, m);
                        typeof(EventManager)
                            .GetMethod(nameof(SubscribeRef))
                            ?.MakeGenericMethod(type)
                            .Invoke(null, new object[]
                            {
                                del,
                                attr.Priority,
                                attr.Once
                            });
                    }
                    catch (System.Exception ex)
                    {
                        App.Log.Log(
                            $"[EventManager] Register Failed to bind ref method[{m.Name}]: {ex.Message}\n{ex.StackTrace}",
                            ELogType.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 从事件系统中注销某个对象的所有事件处理器。
        /// </summary>
        public static void Unregister(object subscriber)
        {
            lock (Lock)
            {
                foreach (var list in Handlers.Values)
                {
                    list.RemoveAll(h =>
                    {
                        if (h.Target.TryGetTarget(out var target))
                        {
                            return ReferenceEquals(target, subscriber);
                        }

                        return true;
                    });
                }
            }

            lock (RefLock)
            {
                foreach (var list in RefHandlers.Values)
                {
                    list.RemoveAll(h =>
                    {
                        if (h.Target.TryGetTarget(out var target))
                        {
                            return ReferenceEquals(target, subscriber);
                        }

                        return true;
                    });
                }
            }
        }
    }
}