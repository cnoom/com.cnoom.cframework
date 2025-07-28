using System;
using CnoomFrameWork.Base.Log;

namespace CnoomFrameWork.Base
{
    /// <summary>
    /// 默认异常处理器
    /// </summary>
    public class DefaultExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// 判断是否可以处理指定异常
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <returns>是否可以处理</returns>
        public bool CanHandle(Exception exception)
        {
            // 默认处理器可以处理所有异常
            return true;
        }

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="context">异常上下文</param>
        /// <returns>是否已成功处理</returns>
        public bool HandleException(ExceptionContext context)
        {
            // 记录异常日志
            LogManager.Error("DefaultExceptionHandler", "处理异常", context.Exception);
            
            // 默认处理器只记录日志，不阻止异常传播
            return false;
        }
    }
}