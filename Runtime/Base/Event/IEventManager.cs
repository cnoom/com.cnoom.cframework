using System;

namespace CnoomFrameWork.Base.Event
{
    /// <summary>
    ///     事件总线，提供线程安全的事件发布/订阅机制
    /// </summary>
    public interface IEventManager
    {
        public void Subscribe<TEvent>(Action<TEvent> handler);
        public void Unsubscribe<TEvent>(Action<TEvent> handler);
        public void Publish<TEvent>(TEvent @event);
        public void AutoRegister(object subscriber);
        public void AutoUnregister(object subscriber);
    }
}