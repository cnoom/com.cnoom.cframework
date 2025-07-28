using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CnoomFrameWork.Base.Log;
using UnityEngine;

namespace CnoomFrameWork.Base
{
    /// <summary>
    /// 异常管理器，负责全局异常处理、捕获和报告
    /// </summary>
    public static class ExceptionManager
    {
        private static readonly List<IExceptionHandler> GlobalHandlers = new();
        private static bool isDevelopmentMode = true;
        private static IExceptionReporter exceptionReporter;

        /// <summary>
        /// 初始化异常管理器
        /// </summary>
        /// <param name="config"></param>
        public static void Initialize(ExceptionConfig config)
        {
            isDevelopmentMode = config.IsDevelopmentMode;
            exceptionReporter = config.ExceptionReporter;

            // 注册全局未处理异常处理
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.logMessageReceived += OnLogMessageReceived;

            LogManager.Info("ExceptionManager", "异常管理器初始化完成");
        }

        /// <summary>
        /// 注册全局异常处理器
        /// </summary>
        /// <param name="handler">异常处理器</param>
        public static void RegisterHandler(IExceptionHandler handler)
        {
            if (handler != null && !GlobalHandlers.Contains(handler))
            {
                GlobalHandlers.Add(handler);
                LogManager.Debug("ExceptionManager", $"注册异常处理器: {handler.GetType().Name}");
            }
        }

        /// <summary>
        /// 取消注册全局异常处理器
        /// </summary>
        /// <param name="handler">异常处理器</param>
        public static void UnregisterHandler(IExceptionHandler handler)
        {
            if (handler != null && GlobalHandlers.Contains(handler))
            {
                GlobalHandlers.Remove(handler);
                LogManager.Debug("ExceptionManager", $"取消注册异常处理器: {handler.GetType().Name}");
            }
        }

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="memberName">调用方法名</param>
        /// <param name="sourceFilePath">源文件路径</param>
        /// <param name="sourceLineNumber">源代码行号</param>
        /// <returns>是否已处理异常</returns>
        public static bool HandleException(
            Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (exception == null)
                return true;

            // 创建异常上下文
            var context = new ExceptionContext
            {
                Exception = exception,
                CallerMember = memberName,
                CallerFile = sourceFilePath,
                CallerLine = sourceLineNumber,
                Variables = CaptureVariables(),
                Timestamp = DateTime.UtcNow,
                StackTrace = new StackTrace(true)
            };

            // 记录异常日志
            LogException(context);

            // 尝试使用全局处理器处理异常
            bool handled = false;
            foreach (var handler in GlobalHandlers)
            {
                try
                {
                    if (handler.CanHandle(exception) && handler.HandleException(context))
                    {
                        handled = true;
                        break;
                    }
                }
                catch (Exception handlerEx)
                {
                    LogManager.Error("ExceptionManager", $"异常处理器执行失败: {handler.GetType().Name}", handlerEx);
                }
            }

            // 如果配置了异常上报接口，则上报异常
            if (exceptionReporter != null)
            {
                try
                {
                    exceptionReporter.ReportException(context);
                }
                catch (Exception reporterEx)
                {
                    LogManager.Error("ExceptionManager", "异常上报失败", reporterEx);
                }
            }

            return handled;
        }

        /// <summary>
        /// 获取格式化的异常信息
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <returns>格式化的异常信息</returns>
        public static string GetFormattedExceptionMessage(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            if (isDevelopmentMode)
            {
                // 开发模式：显示完整堆栈
                return exception.ToString();
            }
            else
            {
                // 生产模式：显示友好提示
                if (exception is BaseFrameworkException frameworkEx)
                {
                    return $"错误 [{frameworkEx.ErrorCode}]: {frameworkEx.Message}";
                }

                return "应用程序遇到了一个问题，请稍后再试。";
            }
        }

        #region 私有方法

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                LogManager.Critical("UnhandledException", "捕获到未处理的异常", exception);
                HandleException(exception);
            }
        }

        private static void OnLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
                // Unity内部异常转换为框架异常
                var exception = new BaseFrameworkException(
                    ErrorCodes.UnityInternalError,
                    $"Unity错误: {logString}",
                    new Exception(stackTrace));

                HandleException(exception);
            }
        }

        private static void LogException(ExceptionContext context)
        {
            var exception = context.Exception;
            var customFields = new Dictionary<string, object>
            {
                ["CallerMember"] = context.CallerMember,
                ["CallerFile"] = context.CallerFile,
                ["CallerLine"] = context.CallerLine
            };

            // 添加变量快照
            if (context.Variables != null && context.Variables.Count > 0)
            {
                customFields["Variables"] = context.Variables;
            }

            if (exception is BaseFrameworkException frameworkEx)
            {
                customFields["ErrorCode"] = frameworkEx.ErrorCode;
                customFields["Timestamp"] = frameworkEx.Timestamp;
            }

            LogManager.Error("ExceptionManager", "异常已捕获", exception, customFields);
        }

        private static Dictionary<string, object> CaptureVariables()
        {
            // 在实际应用中，可以通过反射或其他方式捕获关键变量
            // 这里返回一个空字典作为示例
            return new Dictionary<string, object>();
        }

        #endregion
    }
}