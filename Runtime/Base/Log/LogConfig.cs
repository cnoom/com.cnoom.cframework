using CnoomFrameWork.Base.Config;

namespace CnoomFrameWork.Base.Log
{
    public class LogConfig : IConfig
    {
        public ILog Log { get; set; } = new BaseLog();
    }
}