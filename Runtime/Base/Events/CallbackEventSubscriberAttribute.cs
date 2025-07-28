using System;

namespace CnoomFrameWork.Base.Events
{
    /// <summary>
    ///     用于标记同步回调事件订阅方法。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CallbackEventSubscriberAttribute : Attribute
    {
        public CallbackEventSubscriberAttribute(int priority = 0, bool once = false)
        {
            Priority = priority;
            Once = once;
        }

        public int Priority { get; }
        public bool Once { get; }
    }
}