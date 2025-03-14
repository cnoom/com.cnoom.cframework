using System;
using CnoomFrameWork.Event;
using CnoomFrameWork.Log;

namespace CnoomFrameWork.Core
{
    public interface IModule : IDisposable
    {
        public EventBus EventSystem { get; set; }
        public ILog Log { get; set; }
        void Initialize();
    }
}