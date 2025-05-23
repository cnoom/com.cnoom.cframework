﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CnoomFrameWork.Base.Events
{
    public class EventManager
    {
        /// <summary>
        /// ref 结构体事件处理器委托。
        /// </summary>
        public delegate void RefEventHandler<T>(ref T e) where T : struct;

        private static readonly Dictionary<Type, List<HandlerInfo>> _handlers = new();
        private static readonly Dictionary<Type, List<RefHandlerInfo>> _refHandlers = new();
        private static readonly Dictionary<Type, List<Delegate>> _filters = new();
        private static readonly Dictionary<Type, List<Delegate>> _refFilters = new();
        private static readonly object _lock = new();
        private static readonly object _refLock = new();

        /// <summary>
        /// 注册异步普通事件处理器。
        /// </summary>
        public static void Subscribe<T>(Func<T, Task> handler, int priority = 0, bool once = false)
        {
            AddHandler(typeof(T), handler, priority, once, isAsync: true);
        }

        /// <summary>
        /// 注册同步普通事件处理器。
        /// </summary>
        public static void Subscribe<T>(Action<T> handler, int priority = 0, bool once = false)
        {
            AddHandler(typeof(T), handler, priority, once, isAsync: false);
        }

        /// <summary>
        /// 取消注册普通事件处理器。
        /// </summary>
        public static void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            lock (_lock)
            {
                if (_handlers.TryGetValue(type, out var list))
                {
                    list.RemoveAll(h => h.Handler == (Delegate)handler);
                }
            }
        }

        private static void AddHandler(Type type, Delegate handler, int priority, bool once, bool isAsync)
        {
            lock (_lock)
            {
                if(!_handlers.TryGetValue(type, out var list))
                    list = _handlers[type] = new List<HandlerInfo>();

                list.Add(new HandlerInfo
                {
                    Handler = handler,
                    Priority = priority,
                    Once = once,
                    IsAsync = isAsync,
                    Target = handler.Target
                });
                list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            }
        }

        /// <summary>
        /// 检查事件是否应被特定处理器接收（基于过滤器判断）。
        /// </summary>
        private static bool ShouldInvokeHandler<T>(T e, Delegate handler)
        {
            if(_filters.TryGetValue(typeof(T), out var filterList))
            {
                foreach (Func<T, Delegate, bool> filter in filterList.Cast<Func<T, Delegate, bool>>())
                {
                    if(!filter(e, handler)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查结构体事件是否应被特定处理器接收（基于过滤器判断）。
        /// </summary>
        private static bool ShouldInvokeRefHandler<T>(T e, Delegate handler) where T : struct
        {
            if(_refFilters.TryGetValue(typeof(T), out var filterList))
            {
                foreach (Delegate filterObject in filterList)
                {
                    // 确保过滤器是正确的类型
                    if(filterObject is Func<T, Delegate, bool> filter)
                    {
                        if(!filter(e, handler)) return false;
                    }
                    else
                    {
                        throw new InvalidOperationException($"过滤器类型不匹配: {filterObject.GetType()} 不是 Func<{typeof(T)}, Delegate, bool>");
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 同步或异步发布事件（普通类/结构体）给所有订阅者。
        /// </summary>
        public static async Task PublishAsync<T>(T e)
        {
            List<HandlerInfo> snapshot;
            lock (_lock)
            {
                if(!_handlers.TryGetValue(typeof(T), out var list)) return;
                snapshot = new List<HandlerInfo>(list);
            }

            HashSet<HandlerInfo> toRemove = new();

            foreach (var h in snapshot)
            {
                if(!ShouldInvokeHandler(e, h.Handler)) continue;
                try
                {
                    if(h.IsAsync && h.Handler is Func<T, Task> asyncHandler)
                        await asyncHandler(e).ConfigureAwait(false);
                    else if(h.Handler is Action<T> syncHandler)
                        syncHandler(e);

                    if(h.Once)
                        toRemove.Add(h);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"[EventManager] Error: {ex.Message}");
                }
            }

            if(toRemove.Count > 0)
            {
                lock (_lock)
                {
                    if(_handlers.TryGetValue(typeof(T), out var list))
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
        public static void SubscribeRef<T>(RefEventHandler<T> handler, int priority = 0, bool once = false) where T : struct
        {
            var type = typeof(T);
            lock (_refLock)
            {
                if(!_refHandlers.TryGetValue(type, out var list))
                    list = _refHandlers[type] = new List<RefHandlerInfo>();

                list.Add(new RefHandlerInfo
                {
                    Handler = handler,
                    Priority = priority,
                    Once = once,
                    Target = handler.Target
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
            lock (_refLock)
            {
                if(_refHandlers.TryGetValue(type, out var list))
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
            lock (_refLock)
            {
                if(!_refHandlers.TryGetValue(type, out var list)) return;
                snapshot = new List<RefHandlerInfo>(list);
            }

            HashSet<RefHandlerInfo> toRemove = new();

            foreach (var h in snapshot)
            {
                if(!ShouldInvokeRefHandler(e, h.Handler)) continue;

                if(h.Handler is RefEventHandler<T> handler)
                {
                    handler(ref e);
                    if(h.Once)
                        toRemove.Add(h);
                }
            }

            if(toRemove.Count > 0)
            {
                lock (_refLock)
                {
                    if(_refHandlers.TryGetValue(type, out var list))
                    {
                        foreach (var r in toRemove)
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
            lock (_lock)
            {
                if(!_filters.TryGetValue(type, out var list))
                    list = _filters[type] = new List<Delegate>();
                list.Add(filter);
            }
        }

        /// <summary>
        /// 添加结构体事件过滤器。每个处理器会独立判断是否继续传播。
        /// </summary>
        public static void AddRefFilter<T>(Func<T, Delegate, bool> filter) where T : struct
        {
            var type = typeof(T);
            lock (_refLock)
            {
                if(!_refFilters.TryGetValue(type, out var list))
                    list = _refFilters[type] = new List<Delegate>();
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
                foreach (var attr in m.GetCustomAttributes<EventSubscriberAttribute>())
                {
                    var type = attr.EventType;
                    try
                    {
                        Delegate del = attr.IsAsync
                            ? Delegate.CreateDelegate(typeof(Func<,>).MakeGenericType(type, typeof(Task)), subscriber, m)
                            : Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(type), subscriber, m);

                        AddHandler(type, del, attr.Priority, attr.Once, attr.IsAsync);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine($"[Register] Failed to bind method {m.Name}: {ex.Message}");
                    }
                }

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
                        Console.WriteLine($"[Register] Failed to bind ref method {m.Name}: {ex.Message}");
                    }
                }
            }
        }
        
        /// <summary>
        /// 从事件系统中注销某个对象的所有事件处理器。
        /// </summary>
        public static void Unregister(object subscriber)
        {
            lock (_lock)
            {
                foreach (var list in _handlers.Values)
                {
                    list.RemoveAll(h => h.Target == subscriber);
                }
            }

            lock (_refLock)
            {
                foreach (var list in _refHandlers.Values)
                {
                    list.RemoveAll(h => h.Target == subscriber);
                }
            }
        }
    }
}