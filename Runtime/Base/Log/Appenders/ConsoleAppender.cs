using UnityEngine;

namespace CnoomFrameWork.Base.Log.Appenders
{
    /// <summary>
    /// 控制台输出器
    /// </summary>
    public class ConsoleAppender : ILogAppender
    {
        private readonly ILogFormatter formatter;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formatter">日志格式化器</param>
        public ConsoleAppender(ILogFormatter formatter)
        {
            this.formatter = formatter;
        }

        /// <summary>
        /// 写入日志到控制台
        /// </summary>
        /// <param name="entry">日志条目</param>
        public void Write(LogEntry entry)
        {
            string message = formatter.Format(entry);
            switch (entry.Level)
            {
                case LogLevel.Debug:
                case LogLevel.Info:
                    Debug.Log(message);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    Debug.LogError(message);
                    break;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 控制台输出器无需释放资源
        }
    }
}