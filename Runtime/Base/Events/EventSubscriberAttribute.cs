using System;

namespace CnoomFrameWork.Base.Events
{
    /// <summary>
    /// 用于标记普通事件订阅方法。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventSubscriberAttribute : Attribute
    {
        public Type EventType { get; }
        public int Priority { get; }
        public bool Once { get; }
        public bool IsAsync { get; }

        public EventSubscriberAttribute(Type eventType, int priority = 0, bool once = false, bool isAsync = false)
        {
            EventType = eventType;
            Priority = priority;
            Once = once;
            IsAsync = isAsync;
        }
    }
}