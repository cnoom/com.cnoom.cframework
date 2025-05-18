using System;
using CnoomFrameWork.Base.Log;

namespace CnoomFrameWork.Core
{
    public interface IModule : IDisposable
    {
        public ILog Log { get; set; }
        void Initialize();
    }
}