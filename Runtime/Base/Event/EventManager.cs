using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CnoomFrameWork.Core.Base.Pool;
using CnoomFrameWork.Core.CommonEnum;
using CnoomFrameWork.Extensions;
using UnityEngine.XR;

namespace CnoomFrameWork.Base.Event
{
    public class Handler : IComparable<Handler>
    {
        public Delegate Delegate { get; set; }
        public int Priority { get; set; }
        
        internal Handler(Delegate @delegate, int priority = 0)
        {
            Delegate = @delegate;
            Priority = priority;
        }
        public int CompareTo(Handler other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }
    
    /// <summary>
    ///     事件总线，提供线程安全的事件发布/订阅机制
    /// </summary>
    /// <remarks>
    ///     使用锁机制保证多线程环境下的事件处理安全
    /// </remarks>
    public class EventManager : IEventManager
    {
        private static Dictionary<Type,MethodInfo[]> subscribeMethods = new Dictionary<Type, MethodInfo[]>();
        private readonly Dictionary<Type, List<Handler>> _handlers = new Dictionary<Type, List<Handler>>();
        private readonly object @lock = new object();
        private ObjectPool<Handler> handlerPool;

        public EventManager()
        {
            handlerPool = new ObjectPool<Handler>(
                () => new Handler(null),
                null,handler => handler.Delegate = null,null,100);
        }

        /// <summary>
        ///     订阅指定类型的事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="delegate">事件处理委托</param>
        /// <param name="priority">处理优先级</param>
        public void Subscribe<TEvent>(Action<TEvent> @delegate,int priority = 0)
        {
            Type eventType = typeof(TEvent);
            lock (@lock)
            {
                if(!_handlers.ContainsKey(eventType))
                    _handlers[eventType] = new List<Handler>();
                var handler = handlerPool.Get();
                handler.Delegate = @delegate;
                handler.Priority = priority;
                _handlers[eventType].Add(handler);
                _handlers[eventType].SortByInsertionExtension(SortOrder.Descending);
            }
        }

        /// <summary>
        ///     取消订阅指定类型的事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="handler">要移除的事件处理委托</param>
        public void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            Type eventType = typeof(TEvent);
            lock (@lock)
            {
                if(!_handlers.TryGetValue(eventType, out List<Handler> handlers)) return;
                Handler[] array = handlers.ToArray();
                for(int i = array.Length - 1; i >= 0; i--)
                {
                    Handler h = array[i];
                    if(h.Delegate.Target == handler.Target && h.Delegate.Method == handler.Method)
                    {
                        handlers.Remove(h);
                        handlerPool.Release(h);
                    }
                }
            }
        }

        /// <summary>
        ///     发布事件到所有订阅者
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="event">事件实例</param>
        public void Publish<TEvent>(TEvent @event)
        {
            List<System.Exception> exceptions = new List<System.Exception>(); // 存储所有异常
            Handler[] delegatesCopy;

            lock (@lock)
            {
                if (!_handlers.TryGetValue(typeof(TEvent), out List<Handler> delegates)) return;
                delegatesCopy = delegates.ToArray();  // 创建副本避免并发修改
            }

            foreach (Handler handler in delegatesCopy)
            {
                try
                {
                    (handler.Delegate as Action<TEvent>)?.Invoke(@event);
                }
                catch (System.Exception e)
                {
                    exceptions.Add(e); // 记录所有异常
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException("One or more event handlers failed.", exceptions); // 抛出包含所有异常的聚合异常
            }
        }

        /// <summary>
        ///     自动注册带有[Subscribe]特性的方法作为事件处理程序
        /// </summary>
        /// <param name="subscriber">包含订阅方法的对象实例</param>
        /// <exception cref="ArgumentException">当方法签名不符合要求时</exception>
        /// <example>
        ///     class Subscriber {
        ///     [Subscribe]
        ///     private void OnEvent(MyEvent e) { /* 处理逻辑 */ }
        ///     }
        /// </example>
        public void AutoSubscribe(object subscriber)
        {
            MethodInfo[] methods = GetSubscribeMethods(subscriber.GetType());

            foreach (MethodInfo method in methods)
            {
                ParameterInfo[] parameters = method.GetParameters();
                if(parameters.Length != 1) continue;

                Type eventType = parameters[0].ParameterType;
                SubscribeAttribute subscribe = method.GetCustomAttribute<SubscribeAttribute>();

                Delegate @delegate = Delegate.CreateDelegate(
                    typeof(Action<>).MakeGenericType(eventType),
                    subscriber,
                    method);

                lock (@lock)
                {
                    if(!_handlers.ContainsKey(eventType))
                        _handlers[eventType] = new List<Handler>();
                    Handler handler = handlerPool.Get();
                    handler.Delegate = @delegate;
                    handler.Priority = subscribe.Priority;
                    _handlers[eventType].Add(handler);
                }
            }
        }

        /// <summary>
        ///     自动取消注册带有[Subscribe]特性的方法
        /// </summary>
        /// <param name="subscriber">包含订阅方法的对象实例</param>
        /// <remarks>
        ///     应在对象销毁前调用以避免内存泄漏
        /// </remarks>
        public void AutoUnSubscribe(object subscriber)
        {
            MethodInfo[] methods = GetSubscribeMethods(subscriber.GetType());
            foreach (MethodInfo method in methods)
            {
                ParameterInfo[] parameters = method.GetParameters();
                if(parameters.Length != 1) continue;
                Type eventType = parameters[0].ParameterType;
                Delegate handler = Delegate.CreateDelegate(
                    typeof(Action<>).MakeGenericType(eventType),
                    subscriber,
                    method);

                lock (@lock)
                {
                    if(_handlers.TryGetValue(eventType, out List<Handler> handlers))
                    {
                        handlers.RemoveAll(d => d.Delegate.Target == handler.Target && d.Delegate.Method == handler.Method);
                    }
                }
            }
        }

        /// <summary>
        ///     获取指定类型的具有订阅特性的方法数组
        /// </summary>
        /// <param name="type">指定的类型</param>
        /// <returns></returns>
        private MethodInfo[] GetSubscribeMethods(Type type)
        {
            if(!subscribeMethods.TryGetValue(type, out MethodInfo[] methods))
            {
                methods = type.GetMethods(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.GetCustomAttribute<SubscribeAttribute>() != null).ToArray();
                subscribeMethods.Add(type, methods);
            }
            return methods;
        }
    }
}