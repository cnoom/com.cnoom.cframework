using System;

namespace CnoomFrameWork.Base.IoC
{
    public interface IIoCContainer
    {
        IIoCContainer Bind<TInterface, TImplementation>(ELifecycleType eLifecycle = ELifecycleType.Transient)
            where TImplementation : TInterface;

        IIoCContainer Bind<TInterface>(Type implementationType, ELifecycleType eLifecycle = ELifecycleType.Transient);

        IIoCContainer BindInstance<TInterface>(TInterface instance);

        IIoCContainer UnBind<TInterface>();

        IIoCContainer UnBindInstance<TInterface>(TInterface instance = default);

        IIoCContainer ClearLifecycle(ELifecycleType lifecycle);


        object Resolve(Type type);

        T Resolve<T>();

        void InjectDependencies(object instance);
    }
}