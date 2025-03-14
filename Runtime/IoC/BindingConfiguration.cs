using System;

namespace CnoomFrameWork.IoC
{
    public class BindingConfiguration
    {
        public BindingConfiguration(Type interfaceType, Func<object> factory)
        {
            InterfaceType = interfaceType;
            ImplementationType = interfaceType;
            Factory = factory;
        }

        /// <summary>
        ///     接口类型
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        ///     实现类型
        /// </summary>
        public Type ImplementationType { get; private set; }

        public Func<object> Factory { get; private set; }
        public LifecycleScope Lifecycle { get; private set; }

        /// <summary>
        ///     指定接口的具体实现类型
        /// </summary>
        /// <typeparam name="TImplementation">实现类型必须为类且实现当前接口</typeparam>
        public BindingConfiguration To<TImplementation>() where TImplementation : class
        {
            return To(typeof(TImplementation));
        }

        public BindingConfiguration To(Type implementationType)
        {
            ImplementationType = implementationType;
            return this;
        }

        public BindingConfiguration In(LifecycleScope lifecycle)
        {
            Lifecycle = lifecycle;
            return this;
        }

        public BindingConfiguration AsFactory(Func<object> factory)
        {
            Factory = factory;
            return this;
        }

        public override string ToString()
        {
            return $"BindingConfiguration[{InterfaceType} -> {ImplementationType} ({Lifecycle})]";
        }
    }
}