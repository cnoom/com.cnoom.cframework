using System;

namespace CnoomFrameWork.Base.Events
{
    internal class HandlerInfo
    {
        public Delegate Handler;
        public int Priority;
        public bool Once;
        public WeakReference<object> Target;
        public bool IsAsync;
    }
}