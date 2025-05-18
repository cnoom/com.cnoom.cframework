using CnoomFrameWork.Base.IoC;
using CnoomFrameWork.Base.Log;

namespace CnoomFrameWork.Core
{
    public abstract class Module : IModule
    {
        protected App App;
        [Inject] public ILog Log { set; get; }
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