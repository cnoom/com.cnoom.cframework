using System;

namespace CnoomFrameWork.Core
{
    public class AService : IService
    {
        public Action Initialize => Init;

        protected virtual void Init()
        {
        }
        
        public virtual void Dispose()
        {
            
        }
    }
}