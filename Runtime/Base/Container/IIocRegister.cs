using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Base.Config;

namespace CnoomFrameWork.Base.Container
{
    /// <summary>
    ///     用于配置使用IoC容器的类，通常用于绑定依赖项。
    /// </summary>
    public interface IIocRegister
    {
        int Order { get; set; }
        object Register(BaseContainer container);
    }

    public struct IocRegister<TInterface, TInstance> : IIocRegister where TInstance : TInterface
    {
        public int Order { get; set; }

        public IocRegister(int order)
        {
            Order = order;
        }

        public object Register(BaseContainer container)
        {
            var instance = InstanceFactory.CreateInstance<TInstance>(container);
            container.BindSingleton<TInterface, TInstance>(instance);
            return instance;
        }
    }


    public class IocRegisterConfig : IConfig
    {
        private readonly List<IIocRegister> registers = new();
        public List<IIocRegister> Registers => registers.OrderByDescending(r => r.Order).ToList();

        public void Register<TInterface, TRegister>(int order = 0) where TRegister : TInterface
        {
            registers.Add(new IocRegister<TInterface, TRegister>(order));
        }

        public void Register<TInterface>(int order = 0) where TInterface : class
        {
            registers.Add(new IocRegister<TInterface, TInterface>(order));
        }
    }
}