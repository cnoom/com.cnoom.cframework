namespace CnoomFrameWork.Core
{
    public abstract class Module : IModule
    {
        protected App App;

        public void Initialize()
        {
            App = App.Instance;
            OnInitialize();
        }

        public virtual void Dispose()
        {
        }

        protected virtual void OnInitialize()
        {
        }
    }
}