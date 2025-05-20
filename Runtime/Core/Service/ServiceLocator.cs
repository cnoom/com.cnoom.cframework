using CnoomFrameWork.Base.Config;
using CnoomFrameWork.Base.Events;
using CnoomFrameWork.Base.Container;

namespace CnoomFrameWork.Core
{
    public class ServiceLocator
    {
        private RootContainer _rootContainer;

        internal ServiceLocator(RootContainer rootContainer)
        {
            _rootContainer = rootContainer;
        }

        public void RegisterService<TInterface, TService>() where TInterface : class, IService where TService : TInterface
        {
            TService service = InstanceFactory.CreateInstance<TService>(_rootContainer);
            _rootContainer.BindSingleton<TInterface,TService>(service);
            RegiterService(service);
        }

        public void UnRegisterService<TInterface>() where TInterface : class, IService
        {
            TInterface service = _rootContainer.Resolve<TInterface>();
            EventManager.Unregister(service);
            _rootContainer.UnBindSingleton<TInterface>();
        }
        
        public TInterface GetService<TInterface>() where TInterface : class, IService
        {
            return _rootContainer.Resolve<TInterface>();
        }

        private void RegiterService(IService service)
        {
            EventManager.Register(service);
        }

        internal void AutoRegister()
        {
            ServiceConfig config = ConfigManager.Instance.GetConfig<ServiceConfig>();
            foreach (IIocRegister handler in config.Registers)
            {
                var service = handler.Register(_rootContainer);
                if (service is not IService iService) continue;
                RegiterService(iService);
            }
        }
    }
}