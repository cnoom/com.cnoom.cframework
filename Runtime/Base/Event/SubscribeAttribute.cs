using System;

namespace CnoomFrameWork.Base.Event
{
    /// <summary>
    ///     自动订阅事件
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SubscribeAttribute : Attribute
    {
        public int Priority { get; set; } = 0;
    }
}