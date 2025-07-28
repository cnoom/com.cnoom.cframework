using System;
using System.IO;
using System.Text;

namespace CnoomFrameWork.Base.Log.Appenders
{
    /// <summary>
    /// 文件输出器，支持按日期分割
    /// </summary>
    public class FileAppender : ILogAppender
    {
        private readonly ILogFormatter formatter;
        private readonly string logDirectory;
        private StreamWriter streamWriter;
        private string currentLogDate;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formatter">日志格式化器</param>
        /// <param name="logDirectory">日志文件存放目录</param>
        public FileAppender(ILogFormatter formatter, string logDirectory)
        {
            this.formatter = formatter;
            this.logDirectory = logDirectory;

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        /// <summary>
        /// 写入日志到文件
        /// </summary>
        /// <param name="entry">日志条目</param>
        public void Write(LogEntry entry)
        {
            CheckAndRotateFile(entry.Timestamp);
            
            if (streamWriter != null)
            {
                var message = formatter.Format(entry);
                streamWriter.WriteLine(message);
                streamWriter.Flush();
            }
        }

        private void CheckAndRotateFile(DateTime timestamp)
        {
            string dateString = timestamp.ToString("yyyy-MM-dd");
            if (streamWriter == null || currentLogDate != dateString)
            {
                CloseStream();

                currentLogDate = dateString;
                string filePath = Path.Combine(logDirectory, $"{currentLogDate}.log");
                
                try
                {
                    // 使用UTF-8编码，追加模式
                    streamWriter = new StreamWriter(filePath, true, Encoding.UTF8)
                    {
                        AutoFlush = true
                    };
                }
                catch (Exception ex)
                {
                    // 如果文件无法打开，可以在控制台打印一个错误
                    UnityEngine.Debug.LogError($"[FileAppender] Failed to open log file {filePath}. Reason: {ex.Message}");
                    streamWriter = null;
                }
            }
        }

        private void CloseStream()
        {
            if (streamWriter != null)
            {
                try
                {
                    streamWriter.Flush();
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
                catch
                {
                    // 忽略关闭时可能发生的异常
                }
                streamWriter = null;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            CloseStream();
        }
    }
}