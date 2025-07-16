using CnoomFrameWork.Base.Config;
using CnoomFrameWork.Base.Container;
using CnoomFrameWork.Base.Events;

namespace CnoomFrameWork.Core
{
    public class ServiceLocator
    {
        private readonly RootContainer _rootContainer;

        internal ServiceLocator(RootContainer rootContainer)
        {
            _rootContainer = rootContainer;
        }

        public void RegisterService<TInterface, TService>()
            where TInterface : class, IService where TService : TInterface
        {
            var service = InstanceFactory.CreateInstance<TService>(_rootContainer);
            _rootContainer.BindSingleton<TInterface, TService>(service);
            RegiterService(service);
        }

        public void UnRegisterService<TInterface>() where TInterface : class, IService
        {
            var service = _rootContainer.Resolve<TInterface>();
            EventManager.Unregister(service);
            service.Dispose();
            _rootContainer.UnBindSingleton<TInterface>();
        }

        public TInterface GetService<TInterface>() where TInterface : class, IService
        {
            return _rootContainer.Resolve<TInterface>();
        }

        private void RegiterService(IService service)
        {
            EventManager.Register(service);
            service.Initialize();
        }

        internal void AutoRegister()
        {
            var config = ConfigManager.Instance.GetConfig<ServiceConfig>();
            foreach (var handler in config.Registers)
            {
                var service = handler.Register(_rootContainer);
                if (service is not IService iService) continue;
                RegiterService(iService);
            }
        }
    }
}