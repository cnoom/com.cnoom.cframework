using System;

namespace CnoomFrameWork.Core
{
    public interface IService : IDisposable
    {
        Action Initialize { get; }
    }
}