using System;

namespace CnoomFrameWork.Core
{
    public interface IModule : IDisposable
    {
        void Initialize();
    }
}