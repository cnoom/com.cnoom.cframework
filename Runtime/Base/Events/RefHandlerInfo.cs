using System;

namespace CnoomFrameWork.Base.Events
{
    internal class RefHandlerInfo
    {
        public Delegate Handler;
        public int Priority;
        public bool Once;
        public object Target;
    }
}