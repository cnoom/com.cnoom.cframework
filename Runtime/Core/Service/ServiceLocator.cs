using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Base.Config;
using CnoomFrameWork.Base.Event;
using CnoomFrameWork.Base.IoC;
using UnityEngine.Scripting;

namespace CnoomFrameWork.Core
{
    public class ServiceLocator
    {
        [Inject, Preserve]
        private IIoCContainer container;
        [Inject, Preserve]
        private IEventManager eventManager;

        public void RegisterService<TInterface, TService>() where TInterface : class, IService where TService : TInterface
        {
            container.Bind<TInterface, TService>(ELifecycleType.Singleton);
            TInterface service = container.Resolve<TInterface>();
            eventManager.AutoSubscribe(service);
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
                eventManager.AutoSubscribe(handler.Register(container));
            }
        }
    }
}