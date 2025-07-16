using System;

namespace CnoomFrameWork.Base.Events
{
    /// <summary>
    ///     用于标记结构体事件订阅方法（支持 ref）。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RefEventSubscriberAttribute : Attribute
    {
        public RefEventSubscriberAttribute(Type eventType, int priority = 0, bool once = false)
        {
            EventType = eventType;
            Priority = priority;
            Once = once;
        }

        public Type EventType { get; }
        public int Priority { get; }
        public bool Once { get; }
    }
}