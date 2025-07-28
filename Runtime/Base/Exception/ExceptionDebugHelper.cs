using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CnoomFrameWork.Base
{
    /// <summary>
    /// 异常调试辅助类，用于捕获变量快照和提供调试信息
    /// </summary>
    public static class ExceptionDebugHelper
    {
        private static readonly Dictionary<string, object> GlobalVariables = new Dictionary<string, object>();

        /// <summary>
        /// 注册全局变量，这些变量将在异常发生时被捕获
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="value">变量值</param>
        public static void RegisterGlobalVariable(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                return;

            GlobalVariables[name] = value;
        }

        /// <summary>
        /// 取消注册全局变量
        /// </summary>
        /// <param name="name">变量名</param>
        public static void UnregisterGlobalVariable(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (GlobalVariables.ContainsKey(name))
            {
                GlobalVariables.Remove(name);
            }
        }

        /// <summary>
        /// 捕获当前上下文中的变量快照
        /// </summary>
        /// <param name="target">目标对象（可选）</param>
        /// <returns>变量快照字典</returns>
        public static Dictionary<string, object> CaptureVariables(object target = null)
        {
            var snapshot = new Dictionary<string, object>(GlobalVariables);

            // 如果提供了目标对象，尝试捕获其公共属性
            if (target != null)
            {
                try
                {
                    CaptureObjectProperties(target, snapshot);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"捕获对象属性时出错: {ex.Message}");
                }
            }

            // 添加一些系统信息
            snapshot["SystemTime"] = DateTime.Now;
            snapshot["UnityTime"] = Time.time;
            snapshot["FrameCount"] = Time.frameCount;
            snapshot["Platform"] = Application.platform;
            snapshot["UnityVersion"] = Application.unityVersion;
            snapshot["TargetFrameRate"] = Application.targetFrameRate;
            snapshot["MemoryUsage"] = System.GC.GetTotalMemory(false);

            return snapshot;
        }

        /// <summary>
        /// 捕获对象的公共属性
        /// </summary>
        private static void CaptureObjectProperties(object target, Dictionary<string, object> snapshot)
        {
            if (target == null)
                return;

            Type type = target.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                try
                {
                    if (property.CanRead && property.GetIndexParameters().Length == 0)
                    {
                        string key = $"{type.Name}.{property.Name}";
                        object value = property.GetValue(target);
                        snapshot[key] = value;
                    }
                }
                catch (Exception)
                {
                    // 忽略无法读取的属性
                }
            }
        }

        /// <summary>
        /// 获取异常的详细信息，包括内部异常链
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <returns>格式化的异常信息</returns>
        public static string GetDetailedExceptionInfo(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var details = new System.Text.StringBuilder();
            details.AppendLine("异常详情:");
            
            int level = 0;
            Exception currentEx = exception;
            
            while (currentEx != null)
            {
                string indent = new string(' ', level * 2);
                details.AppendLine($"{indent}[Level {level}] {currentEx.GetType().Name}: {currentEx.Message}");
                
                if (currentEx is BaseFrameworkException frameworkEx)
                {
                    details.AppendLine($"{indent}  错误码: {frameworkEx.ErrorCode}");
                    details.AppendLine($"{indent}  时间戳: {frameworkEx.Timestamp:yyyy-MM-dd HH:mm:ss.fff}");
                }
                
                details.AppendLine($"{indent}  堆栈跟踪:");
                details.AppendLine($"{indent}  {currentEx.StackTrace?.Replace("\n", "\n" + indent + "  ")}");
                
                currentEx = currentEx.InnerException;
                level++;
            }
            
            return details.ToString();
        }
    }
}