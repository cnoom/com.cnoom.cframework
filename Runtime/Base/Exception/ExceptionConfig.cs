using CnoomFrameWork.Base.Config;

namespace CnoomFrameWork.Base
{
    public class ExceptionConfig : IConfig
    {
        /// <summary>
        /// 是否为开发模式，在开发模式下会输出详细的异常信息，而在生产模式下会只输出异常的摘要信息。
        /// </summary>
        public bool IsDevelopmentMode { get; set; } = true;
        /// <remarks>
        /// 异常上报接口实现，用于将异常信息上报到指定的目标，如服务器、日志文件等。
        /// </remarks>
        public IExceptionReporter ExceptionReporter { get; set; }
    }
}