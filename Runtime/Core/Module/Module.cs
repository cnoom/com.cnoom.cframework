using CnoomFrameWork.Event;
using CnoomFrameWork.IoC;
using CnoomFrameWork.Log;

namespace CnoomFrameWork.Core
{
    public abstract class Module : IModule
    {
        [Inject] public ILog Log { set; get; }
        [Inject] public EventBus EventSystem { get; set; }
        protected App App;
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