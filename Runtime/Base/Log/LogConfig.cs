using CnoomFrameWork.Base.Config;

namespace CnoomFrameWork.Base.Log
{
    /// <summary>
    /// 日志文件格式
    /// </summary>
    public enum LogFileFormat
    {
        Text,
        Json
    }

    /// <summary>
    /// 日志系统配置
    /// </summary>
    public class LogConfig : IConfig
    {
        /// <summary>
        /// 全局日志级别阈值，低于此级别的日志将被忽略
        /// </summary>
        public LogLevel GlobalLevelThreshold { get; set; } = LogLevel.Info;

        /// <summary>
        /// 是否启用控制台输出
        /// </summary>
        public bool EnableConsoleAppender { get; set; } = true;

        /// <summary>
        /// 是否启用文件输出
        /// </summary>
        public bool EnableFileAppender { get; set; } = false;

        /// <summary>
        /// 文件日志的存储目录 (相对于 Application.persistentDataPath)
        /// </summary>
        public string FileAppenderDirectory { get; set; } = "Logs";

        /// <summary>
        /// 文件日志的格式
        /// </summary>
        public LogFileFormat FileAppenderFormat { get; set; } = LogFileFormat.Text;

        /// <summary>
        /// 是否启用异步日志记录
        /// </summary>
        public bool EnableAsync { get; set; } = true;

        /// <summary>
        /// 是否启用日志压缩归档 (暂未实现)
        /// </summary>
        public bool EnableArchive { get; set; } = false;
    }
}
