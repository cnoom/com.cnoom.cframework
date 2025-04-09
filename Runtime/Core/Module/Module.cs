using CnoomFrameWork.Base.Event;
using CnoomFrameWork.Base.IoC;
using CnoomFrameWork.Base.Log;

namespace CnoomFrameWork.Core
{
    public abstract class Module : IModule
    {
        protected App App;
        [Inject] public ILog Log { set; get; }
        [Inject] public IEventManager EventSystem { get; set; }
        public void Initialize()
        {
            App = App.Instance;
            OnInitialize();
        }

        public virtual void Dispose()
        {
        }

        protected virtual void OnInitialize() { }
    }
}