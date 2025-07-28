using System;

namespace CnoomFrameWork.Base
{
    /// <summary>
    /// 框架中所有自定义异常的基类
    /// </summary>
    public class BaseFrameworkException : Exception
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// 异常发生的时间戳
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">错误消息</param>
        public BaseFrameworkException(int errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">内部异常</param>
        public BaseFrameworkException(int errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// 重写ToString方法，以包含更多信息
        /// </summary>
        /// <returns>格式化的异常信息字符串</returns>
        public override string ToString()
        {
            return $"ErrorCode: {ErrorCode}\nTimestamp: {Timestamp:o}\n{base.ToString()}";
        }
    }
}