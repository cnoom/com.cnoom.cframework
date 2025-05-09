using System;

namespace CnoomFrameWork.Base.Event
{
    /// <summary>
    ///     事件总线，提供线程安全的事件发布/订阅机制
    /// </summary>
    public interface IEventManager
    {
        public void Subscribe<TEvent>(Action<TEvent> @delegate,int priority = 0);
        public void Unsubscribe<TEvent>(Action<TEvent> handler);
        public void Publish<TEvent>(TEvent @event);
        public void AutoSubscribe(object subscriber);
        public void AutoUnSubscribe(object subscriber);
    }
}