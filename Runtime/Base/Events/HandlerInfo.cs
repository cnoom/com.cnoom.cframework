using System;

namespace CnoomFrameWork.Base.Events
{
    internal class HandlerInfo
    {
        public Delegate Handler;
        public bool Once;
        public int Priority;
        public WeakReference<object> Target;
    }
}