using System;
using CnoomFrameWork.Base.Event;
using CnoomFrameWork.Base.Log;


namespace CnoomFrameWork.Core
{
    public interface IModule : IDisposable
    {
        public EventManager EventSystem { get; set; }
        public ILog Log { get; set; }
        void Initialize();
    }
}