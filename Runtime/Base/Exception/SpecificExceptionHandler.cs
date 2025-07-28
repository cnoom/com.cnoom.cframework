using System;
using CnoomFrameWork.Base.Log;
using UnityEngine;

namespace CnoomFrameWork.Base
{
    /// <summary>
    /// 特定类型异常处理器示例
    /// </summary>
    public class SpecificExceptionHandler : IExceptionHandler
    {
        private readonly Type targetExceptionType;
        private readonly Action<ExceptionContext> handlerAction;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="exceptionType">目标异常类型</param>
        /// <param name="handler">处理函数</param>
        public SpecificExceptionHandler(Type exceptionType, Action<ExceptionContext> handler)
        {
            targetExceptionType = exceptionType ?? throw new ArgumentNullException(nameof(exceptionType));
            handlerAction = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        /// 判断是否可以处理指定异常
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <returns>是否可以处理</returns>
        public bool CanHandle(Exception exception)
        {
            return exception != null && targetExceptionType.IsInstanceOfType(exception);
        }

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="context">异常上下文</param>
        /// <returns>是否已成功处理</returns>
        public bool HandleException(ExceptionContext context)
        {
            try
            {
                handlerAction(context);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Error("SpecificExceptionHandler", $"处理 {targetExceptionType.Name} 类型异常时发生错误", ex);
                return false;
            }
        }
    }

    /// <summary>
    /// 特定异常处理器工厂，用于创建针对特定异常类型的处理器
    /// </summary>
    public static class ExceptionHandlerFactory
    {
        /// <summary>
        /// 创建网络异常处理器
        /// </summary>
        /// <returns>异常处理器</returns>
        public static IExceptionHandler CreateNetworkExceptionHandler()
        {
            return new SpecificExceptionHandler(typeof(NetworkException), context =>
            {
                var exception = context.Exception as NetworkException;
                LogManager.Error("NetworkHandler", $"网络异常 [{exception.ErrorCode}]: {exception.Message}");
                
                // 这里可以添加网络重连逻辑
                Debug.Log("尝试重新连接网络...");
            });
        }

        /// <summary>
        /// 创建资源异常处理器
        /// </summary>
        /// <returns>异常处理器</returns>
        public static IExceptionHandler CreateResourceExceptionHandler()
        {
            return new SpecificExceptionHandler(typeof(ResourceException), context =>
            {
                var exception = context.Exception as ResourceException;
                LogManager.Error("ResourceHandler", $"资源异常 [{exception.ErrorCode}]: {exception.Message}");
                
                // 这里可以添加资源重新加载逻辑
                Debug.Log("尝试重新加载资源...");
            });
        }

        /// <summary>
        /// 创建UI异常处理器
        /// </summary>
        /// <returns>异常处理器</returns>
        public static IExceptionHandler CreateUiExceptionHandler()
        {
            return new SpecificExceptionHandler(typeof(UiException), context =>
            {
                var exception = context.Exception as UiException;
                LogManager.Error("UiHandler", $"UI异常 [{exception.ErrorCode}]: {exception.Message}");
                
                // 这里可以添加UI恢复逻辑
                Debug.Log("尝试恢复UI状态...");
            });
        }
    }
}