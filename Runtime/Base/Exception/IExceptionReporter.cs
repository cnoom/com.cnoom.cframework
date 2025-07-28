namespace CnoomFrameWork.Base
{
    /// <summary>
    /// 异常上报接口
    /// </summary>
    public interface IExceptionReporter
    {
        /// <summary>
        /// 上报异常
        /// </summary>
        /// <param name="context">异常上下文</param>
        void ReportException(ExceptionContext context);
    }
}