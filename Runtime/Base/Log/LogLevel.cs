namespace CnoomFrameWork.Base.Log
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 调试信息，用于开发和诊断
        /// </summary>
        Debug = 0,
        /// <summary>
        /// 常规信息，用于追踪应用流程
        /// </summary>
        Info = 1,
        /// <summary>
        /// 警告信息，表示可能的问题
        /// </summary>
        Warning = 2,
        /// <summary>
        /// 错误信息，表示可恢复的错误
        /// </summary>
        Error = 3,
        /// <summary>
        /// 严重错误，可能导致应用终止
        /// </summary>
        Critical = 4,
    }
}