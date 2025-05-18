using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Base.Config;
using CnoomFrameWork.Base.IoC;
using CnoomFrameWork.Base.Events;
using UnityEngine.Scripting;

namespace CnoomFrameWork.Core
{
    public class ServiceLocator
    {
        [Inject, Preserve]
        private IIoCContainer container;

        public void RegisterService<TInterface, TService>() where TInterface : class, IService where TService : TInterface
        {
            container.Bind<TInterface, TService>(ELifecycleType.Singleton);
            TInterface service = container.Resolve<TInterface>();
            RegiterService(service);
        }

        public void UnRegisterService<TInterface>() where TInterface : class, IService
        {
            TInterface service = container.Resolve<TInterface>();
            EventManager.Unregister(service);
            container.UnBind<TInterface>();
            service.OnUnRegister();
        }
        
        public TInterface GetService<TInterface>() where TInterface : class, IService
        {
            return container.Resolve<TInterface>();
        }

        private void RegiterService(IService service)
        {
            EventManager.Register(service);
            service.OnRegister();
        }

        internal void AutoRegister()
        {
            ServiceConfig config = ConfigManager.Instance.GetConfig<ServiceConfig>();
            foreach (IIocRegister handler in config.Registers)
            {
                var service = handler.Register(container);
                if (service is not IService iService) continue;
                RegiterService(iService);
            }
        }
    }
}