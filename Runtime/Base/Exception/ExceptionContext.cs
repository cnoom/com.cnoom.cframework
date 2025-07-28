using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CnoomFrameWork.Base
{
    /// <summary>
    /// 异常上下文，包含异常发生时的相关信息
    /// </summary>
    public class ExceptionContext
    {
        /// <summary>
        /// 异常对象
        /// </summary>
        public Exception Exception { get; set; }
        
        /// <summary>
        /// 调用方法名
        /// </summary>
        public string CallerMember { get; set; }
        
        /// <summary>
        /// 调用文件路径
        /// </summary>
        public string CallerFile { get; set; }
        
        /// <summary>
        /// 调用行号
        /// </summary>
        public int CallerLine { get; set; }
        
        /// <summary>
        /// 异常发生时间
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// 异常发生时的变量快照
        /// </summary>
        public Dictionary<string, object> Variables { get; set; }
        
        /// <summary>
        /// 堆栈跟踪
        /// </summary>
        public StackTrace StackTrace { get; set; }
    }
}