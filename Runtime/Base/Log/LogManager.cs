using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using CnoomFrameWork.Base.Log.Appenders;
using CnoomFrameWork.Base.Log.Formatters;
using UnityEngine;

namespace CnoomFrameWork.Base.Log
{
    /// <summary>
    /// 全局日志管理器，提供高性能的异步日志记录功能
    /// </summary>
    public static class LogManager
    {
        private static readonly List<ILogAppender> Appenders = new List<ILogAppender>();
        private static readonly BlockingCollection<LogEntry> LogQueue = new BlockingCollection<LogEntry>(new ConcurrentQueue<LogEntry>());
        private static readonly Thread WorkerThread;
        private static readonly CancellationTokenSource Cts = new CancellationTokenSource();

        private static LogConfig config;
        private static bool isInitialized = false;

        /// <summary>
        /// 静态构造函数，用于启动后台工作线程
        /// </summary>
        static LogManager()
        {
            WorkerThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "LogWorkerThread"
            };
            WorkerThread.Start();
            Application.quitting += OnApplicationQuitting;
        }

        /// <summary>
        /// 初始化日志系统
        /// </summary>
        /// <param name="logConfig">日志配置</param>
        public static void Initialize(LogConfig logConfig)
        {
            if (isInitialized)
            {
                UnityEngine.Debug.LogWarning("[LogManager] 已初始化，请勿重复调用。");
                return;
            }

            config = logConfig ?? new LogConfig();

            // 清理旧的Appenders
            foreach (var appender in Appenders)
            {
                appender.Dispose();
            }
            Appenders.Clear();

            // 根据配置创建Appenders
            if (config.EnableConsoleAppender)
            {
                Appenders.Add(new ConsoleAppender(new TextFormatter()));
            }

            if (config.EnableFileAppender)
            {
                var logDir = Path.Combine(Application.persistentDataPath, config.FileAppenderDirectory);
                ILogFormatter formatter = config.FileAppenderFormat == LogFileFormat.Json
                    ? new JsonFormatter()
                    : new TextFormatter();
                Appenders.Add(new FileAppender(formatter, logDir));
            }

            isInitialized = true;
            Info("LogManager", "日志系统初始化完成。");
        }

        /// <summary>
        /// 记录日志的核心方法
        /// </summary>
        public static void Log(
            LogLevel level,
            string category,
            string message,
            Exception exception = null,
            IDictionary<string, object> customFields = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!isInitialized || level < config.GlobalLevelThreshold)
            {
                return;
            }

            var entry = new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Category = category,
                Message = message,
                Exception = exception,
                CustomFields = customFields,
                CallerMember = memberName,
                CallerFile = Path.GetFileName(sourceFilePath),
                CallerLine = sourceLineNumber,
                ThreadId = Thread.CurrentThread.ManagedThreadId
            };

            if (config.EnableAsync)
            {
                if (!LogQueue.IsAddingCompleted)
                {
                    LogQueue.Add(entry);
                }
            }
            else
            {
                ProcessEntry(entry);
            }
        }

        private static void ProcessLogQueue()
        {
            try
            {
                foreach (var entry in LogQueue.GetConsumingEnumerable(Cts.Token))
                {
                    ProcessEntry(entry);
                }
            }
            catch (OperationCanceledException)
            {
                // 线程正常退出
            }
        }

        private static void ProcessEntry(LogEntry entry)
        {
            foreach (var appender in Appenders)
            {
                try
                {
                    appender.Write(entry);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"[LogManager] Appender写入失败: {ex}");
                }
            }
        }

        private static void OnApplicationQuitting()
        {
            Info("LogManager", "应用正在退出，处理剩余日志...");
            
            // 停止接受新的日志
            LogQueue.CompleteAdding();

            // 等待工作线程处理完所有日志，设置一个超时时间
            if (!WorkerThread.Join(TimeSpan.FromSeconds(5)))
            {
                UnityEngine.Debug.LogWarning("[LogManager] 日志处理线程超时，可能丢失部分日志。");
            }
            Cts.Cancel();

            foreach (var appender in Appenders)
            {
                appender.Dispose();
            }
        }
        
        // 便捷方法
        public static void Debug(string category, string message, IDictionary<string, object> customFields = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) =>
            Log(LogLevel.Debug, category, message, null, customFields, memberName, sourceFilePath, sourceLineNumber);

        public static void Info(string category, string message, IDictionary<string, object> customFields = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) =>
            Log(LogLevel.Info, category, message, null, customFields, memberName, sourceFilePath, sourceLineNumber);

        public static void Warn(string category, string message, IDictionary<string, object> customFields = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) =>
            Log(LogLevel.Warning, category, message, null, customFields, memberName, sourceFilePath, sourceLineNumber);

        public static void Error(string category, string message, Exception ex = null, IDictionary<string, object> customFields = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) =>
            Log(LogLevel.Error, category, message, ex, customFields, memberName, sourceFilePath, sourceLineNumber);

        public static void Critical(string category, string message, Exception ex = null, IDictionary<string, object> customFields = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) =>
            Log(LogLevel.Critical, category, message, ex, customFields, memberName, sourceFilePath, sourceLineNumber);
    }
}