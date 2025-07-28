namespace CnoomFrameWork.Base.Log
{
    /// <summary>
    /// 日志格式化器接口
    /// </summary>
    public interface ILogFormatter
    {
        /// <summary>
        /// 格式化日志条目
        /// </summary>
        /// <param name="entry">日志条目</param>
        /// <returns>格式化后的字符串</returns>
        string Format(LogEntry entry);
    }
}