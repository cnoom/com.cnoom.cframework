using System;

namespace CnoomFrameWork.Event
{
    /// <summary>
    ///     自动订阅事件
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SubscribeAttribute : Attribute
    {
    }
}