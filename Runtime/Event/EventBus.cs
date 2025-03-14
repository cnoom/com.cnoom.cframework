using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CnoomFrameWork.Event
{
    /// <summary>
    ///     事件总线，提供线程安全的事件发布/订阅机制
    /// </summary>
    /// <remarks>
    ///     使用锁机制保证多线程环境下的事件处理安全
    /// </remarks>
    public class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> handlers = new Dictionary<Type, List<Delegate>>();
        private readonly object @lock = new object();

        /// <summary>
        ///     订阅指定类型的事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="handler">事件处理委托</param>
        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            Type eventType = typeof(TEvent);
            lock (@lock)
            {
                if(!handlers.ContainsKey(eventType))
                    handlers[eventType] = new List<Delegate>();

                handlers[eventType].Add(handler);
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
                if(this.handlers.TryGetValue(eventType, out List<Delegate> handlers))
                {
                    handlers.RemoveAll(d => d.Target == handler.Target && d.Method == handler.Method);
                }
            }
        }

        /// <summary>
        ///     发布事件到所有订阅者
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="event">事件实例</param>
        /// <remarks>
        ///     当事件处理抛出异常时，会捕获第一个异常并重新抛出
        /// </remarks>
        public void Publish<TEvent>(TEvent @event)
        {
            System.Exception firstException = null;
            lock (@lock)
            {
                if(!handlers.TryGetValue(typeof(TEvent), out List<Delegate> delegates)) return;
                // 遍历处理程序时捕获首个异常
                foreach (Delegate handler in delegates)
                {
                    try
                    {
                        (handler as Action<TEvent>)?.Invoke(@event);
                    }
                    catch (System.Exception e)
                    {
                        firstException ??= e;
                    }
                }
            }
            if(firstException != null)
            {
                throw new AggregateException(firstException);
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
        public void AutoRegister(object subscriber)
        {
            IEnumerable<MethodInfo> methods = subscriber.GetType().GetMethods(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<SubscribeAttribute>() != null);

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
                    if(!handlers.ContainsKey(eventType))
                        handlers[eventType] = new List<Delegate>();

                    handlers[eventType].Add(handler);
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
        public void AutoUnregister(object subscriber)
        {
            IEnumerable<MethodInfo> methods = subscriber.GetType().GetMethods(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<SubscribeAttribute>() != null);
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
                    if(this.handlers.TryGetValue(eventType, out List<Delegate> handlers))
                    {
                        handlers.RemoveAll(d => d.Target == handler.Target && d.Method == handler.Method);
                    }
                }
            }
        }
    }
}