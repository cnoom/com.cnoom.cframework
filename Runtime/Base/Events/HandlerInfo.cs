using System;

namespace CnoomFrameWork.Base.Events
{
    internal class HandlerInfo
    {
        public Delegate Handler;
        public int Priority;
        public bool Once;
        public bool IsAsync;
        public object Target;
    }
}