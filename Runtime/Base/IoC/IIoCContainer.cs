using System;

namespace CnoomFrameWork.Base.IoC
{
    public interface IIoCContainer
    {
        void Bind<TInterface, TImplementation>(ELifecycleType eLifecycle = ELifecycleType.Transient)
            where TImplementation : TInterface;

        void Bind<TInterface>(Type implementationType, ELifecycleType eLifecycle = ELifecycleType.Transient);

        void BindInstance<TInterface>(TInterface instance);

        void UnBind<TInterface>();

        void UnBindInstance<TInterface>(TInterface instance = default);

        object Resolve(Type type);

        T Resolve<T>();
    }
}