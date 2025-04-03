using CnoomFrameWork.Base.Config;
using CnoomFrameWork.Core;

namespace CnoomFrameWork.Base.Log
{
    public class LogConfig : IConfig
    {
        public ILog Log { get; set; } = new BaseLog();
    }
}