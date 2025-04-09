using System.Collections.Generic;
using CnoomFrameWork.Base.Config;

namespace CnoomFrameWork.Base.IoC
{
    /// <summary>
    ///     用于配置使用IoC容器的类，通常用于绑定依赖项。
    /// </summary>
    public interface IIocRegister
    {
        int Order { get; set; }
        object Register(IIoCContainer container);
    }

    public struct IocRegister<TInterface, TRegister> : IIocRegister where TRegister : TInterface
    {
        public int Order { get; set; }

        public IocRegister(int order)
        {
            Order = order;
        }

        public object Register(IIoCContainer container)
        {
            container.Bind<TInterface, TRegister>(ELifecycleType.Singleton);
            return container.Resolve<TInterface>();
        }
    }

    public class IocRegisterConfig : IConfig
    {
        public Dictionary<int, IIocRegister> Registers { get; } = new Dictionary<int, IIocRegister>();

        public void Register<TInterface, TRegister>(int order = 0) where TRegister : TInterface
        {
            Registers.Add(order, new IocRegister<TInterface, TRegister>(order));
        }

        public void Register<TInterface>(int order = 0) where TInterface : class
        {
            Registers.Add(order, new IocRegister<TInterface, TInterface>(order));
        }
    }
}