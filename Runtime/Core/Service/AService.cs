using System;

namespace CnoomFrameWork.Core
{
    /// <summary>
    /// 抽象服务类，实现接口的初始化和释放方法。
    /// </summary>
    public class AService : IService
    {
        
        public virtual void Initialize()
        {
        }
        
        public virtual void Dispose()
        {
            
        }
    }
}