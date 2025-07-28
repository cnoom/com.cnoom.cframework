using System;
using System.Collections.Generic;

namespace CnoomFrameWork.Base.Log
{
    /// <summary>
    /// 结构化的日志条目
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// 模块/分类名称
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 调用者文件名
        /// </summary>
        public string CallerFile { get; set; }

        /// <summary>
        /// 调用者行号
        /// </summary>
        public int CallerLine { get; set; }

        /// <summary>
        /// 调用者成员名 (函数/方法)
        /// </summary>
        public string CallerMember { get; set; }

        /// <summary>
        /// 线程ID
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// 关联的异常信息
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 用户自定义字段
        /// </summary>
        public IDictionary<string, object> CustomFields { get; set; }
    }
}