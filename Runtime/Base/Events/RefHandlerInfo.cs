using System;

namespace CnoomFrameWork.Base.Events
{
    internal class RefHandlerInfo
    {
        public Delegate Handler;
        public bool Once;
        public int Priority;
        public WeakReference<object> Target;
    }
}