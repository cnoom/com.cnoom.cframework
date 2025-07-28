using System;

namespace CnoomFrameWork.Base.Log
{
    /// <summary>
    /// 日志输出器接口
    /// </summary>
    public interface ILogAppender : IDisposable
    {
        /// <summary>
        /// 写入日志条目
        /// </summary>
        /// <param name="entry">日志条目</param>
        void Write(LogEntry entry);
    }
}