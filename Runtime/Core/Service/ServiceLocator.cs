using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Base.Config;
using CnoomFrameWork.Base.Event;
using CnoomFrameWork.Base.IoC;


namespace CnoomFrameWork.Core
{
    public class ServiceLocator
    {
        private IIoCContainer container;
        private IEventManager eventManager;

        internal ServiceLocator(IIoCContainer container, IEventManager eventManager)
        {
            this.container = container;
            this.eventManager = eventManager;
        }

        public void RegisterService<TInterface, TService>() where TInterface : class, IService where TService : TInterface
        {
            container.Bind<TInterface, TService>(ELifecycleType.Singleton);
            TInterface service = container.Resolve<TInterface>();
            eventManager.AutoRegister(service);
            service.OnRegister();
        }

        public void UnRegisterService<TInterface>() where TInterface : class, IService
        {
            TInterface service = container.Resolve<TInterface>();
            container.UnBind<TInterface>();
            service.OnUnRegister();
        }

        internal void AutoRegister()
        {
            ServiceConfig config = ConfigManager.Instance.GetConfig<ServiceConfig>();
            IOrderedEnumerable<KeyValuePair<int, IIocRegister>> list = config.Registers.OrderByDescending(x => x.Key);
            foreach ((int _, IIocRegister handler) in list)
            {
                eventManager.AutoRegister(handler.Register(container));
            }
        }
    }
}