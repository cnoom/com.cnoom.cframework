using System;

namespace CnoomFrameWork.Core
{
    /// <summary>
    ///     自动注册应用模块特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoRegisterModuleAttribute : Attribute
    {
        /// <summary>
        ///     权重，权重大的优先注册
        /// </summary>
        public int Order = 0;
    }
}