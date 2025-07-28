using System;

namespace CnoomFrameWork.Base
{
    /// <summary>
    /// 异常处理器接口
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// 判断是否可以处理指定异常
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <returns>是否可以处理</returns>
        bool CanHandle(Exception exception);

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="context">异常上下文</param>
        /// <returns>是否已成功处理</returns>
        bool HandleException(ExceptionContext context);
    }
}