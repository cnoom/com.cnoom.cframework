using System;
using System.Collections.Generic;
using System.Text;
using CnoomFrameWork.Base.Log;
using UnityEngine;

namespace CnoomFrameWork.Base
{
    /// <summary>
    /// 默认异常上报实现
    /// </summary>
    public class DefaultExceptionReporter : IExceptionReporter
    {
        private readonly string reportEndpoint;
        private readonly bool logLocally;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reportEndpoint">上报服务端点（可选）</param>
        /// <param name="logLocally">是否在本地记录日志</param>
        public DefaultExceptionReporter(string reportEndpoint = null, bool logLocally = true)
        {
            this.reportEndpoint = reportEndpoint;
            this.logLocally = logLocally;
        }

        /// <summary>
        /// 上报异常
        /// </summary>
        /// <param name="context">异常上下文</param>
        public void ReportException(ExceptionContext context)
        {
            if (context == null || context.Exception == null)
                return;

            // 在本地记录详细日志
            if (logLocally)
            {
                LogExceptionLocally(context);
            }

            // 如果配置了上报端点，则上报到服务器
            if (!string.IsNullOrEmpty(reportEndpoint))
            {
                ReportToServer(context);
            }
        }

        /// <summary>
        /// 在本地记录异常日志
        /// </summary>
        private void LogExceptionLocally(ExceptionContext context)
        {
            var customFields = new Dictionary<string, object>
            {
                ["CallerMember"] = context.CallerMember,
                ["CallerFile"] = context.CallerFile,
                ["CallerLine"] = context.CallerLine,
                ["Timestamp"] = context.Timestamp
            };

            // 添加变量快照
            if (context.Variables != null && context.Variables.Count > 0)
            {
                customFields["Variables"] = FormatVariables(context.Variables);
            }

            LogManager.Critical("ExceptionReporter", "异常已上报", context.Exception, customFields);
        }

        /// <summary>
        /// 上报异常到服务器
        /// </summary>
        private void ReportToServer(ExceptionContext context)
        {
            try
            {
                // 构建上报数据
                var reportData = new Dictionary<string, object>
                {
                    ["timestamp"] = context.Timestamp.ToString("o"),
                    ["exceptionType"] = context.Exception.GetType().FullName,
                    ["message"] = context.Exception.Message,
                    ["stackTrace"] = context.Exception.StackTrace,
                    ["callerInfo"] = new Dictionary<string, object>
                    {
                        ["member"] = context.CallerMember,
                        ["file"] = context.CallerFile,
                        ["line"] = context.CallerLine
                    }
                };

                // 添加错误码和时间戳（如果是框架异常）
                if (context.Exception is BaseFrameworkException frameworkEx)
                {
                    reportData["errorCode"] = frameworkEx.ErrorCode;
                }

                // 添加变量快照
                if (context.Variables != null && context.Variables.Count > 0)
                {
                    reportData["variables"] = context.Variables;
                }

                // 添加内部异常信息
                if (context.Exception.InnerException != null)
                {
                    var innerExceptions = new List<Dictionary<string, string>>();
                    var innerEx = context.Exception.InnerException;
                    while (innerEx != null)
                    {
                        innerExceptions.Add(new Dictionary<string, string>
                        {
                            ["type"] = innerEx.GetType().FullName,
                            ["message"] = innerEx.Message,
                            ["stackTrace"] = innerEx.StackTrace
                        });
                        innerEx = innerEx.InnerException;
                    }
                    reportData["innerExceptions"] = innerExceptions;
                }

                // 添加系统信息
                reportData["systemInfo"] = new Dictionary<string, object>
                {
                    ["platform"] = Application.platform.ToString(),
                    ["unityVersion"] = Application.unityVersion,
                    ["productName"] = Application.productName,
                    ["version"] = Application.version,
                    ["deviceModel"] = SystemInfo.deviceModel,
                    ["deviceName"] = SystemInfo.deviceName,
                    ["operatingSystem"] = SystemInfo.operatingSystem,
                    ["processorType"] = SystemInfo.processorType,
                    ["systemMemorySize"] = SystemInfo.systemMemorySize,
                    ["graphicsDeviceName"] = SystemInfo.graphicsDeviceName
                };

                // 序列化为JSON
                string jsonData = JsonUtility.ToJson(reportData);

                // 实际项目中，这里应该使用HTTP客户端发送数据到服务器
                // 例如使用UnityWebRequest或其他HTTP客户端
                // 这里仅作为示例，打印日志
                LogManager.Debug("ExceptionReporter", $"模拟上报异常到服务器: {reportEndpoint}");
                
                // 在实际项目中，可以使用以下代码发送HTTP请求
                /*
                UnityWebRequest request = new UnityWebRequest(reportEndpoint, "POST");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                
                // 发送请求
                var operation = request.SendWebRequest();
                operation.completed += (op) => {
                    if (request.result == UnityWebRequest.Result.Success) {
                        LogManager.Debug("ExceptionReporter", "异常上报成功");
                    } else {
                        LogManager.Error("ExceptionReporter", $"异常上报失败: {request.error}");
                    }
                    request.Dispose();
                };
                */
            }
            catch (Exception ex)
            {
                LogManager.Error("ExceptionReporter", "上报异常时发生错误", ex);
            }
        }

        /// <summary>
        /// 格式化变量快照
        /// </summary>
        private string FormatVariables(Dictionary<string, object> variables)
        {
            if (variables == null || variables.Count == 0)
                return "{}";

            var sb = new StringBuilder();
            sb.AppendLine("{");
            
            foreach (var pair in variables)
            {
                string value = pair.Value?.ToString() ?? "null";
                sb.AppendLine($"  {pair.Key}: {value}");
            }
            
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}