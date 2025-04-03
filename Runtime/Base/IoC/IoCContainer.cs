using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CnoomFrameWork.Base.IoC
{
    public class IoCContainer : IIoCContainer
    {
        private class Registration
        {
            public Type ImplementationType { get; set; }
            public object Instance { get; set; }
            public ELifecycleType ELifecycle { get; set; }
        }

        private readonly Dictionary<Type, Registration> _registrations = new();

        public IIoCContainer Bind<TInterface, TImplementation>(ELifecycleType eLifecycle = ELifecycleType.Transient)
            where TImplementation : TInterface => Bind<TInterface>(typeof(TImplementation), eLifecycle);
        
        public IIoCContainer Bind<TInterface>(Type implementationType, ELifecycleType eLifecycle = ELifecycleType.Transient)
        {
            var interfaceType = typeof(TInterface);
            _registrations[interfaceType] = new Registration
            {
                ImplementationType = implementationType,
                ELifecycle = eLifecycle
            };
            return this;
        }

        public IIoCContainer BindInstance<TInterface>(TInterface instance)
        {
            var interfaceType = typeof(TInterface);
            _registrations[interfaceType] = new Registration
            {
                Instance = instance,
                ELifecycle = ELifecycleType.Singleton
            };
            return this;
        }

        public IIoCContainer UnBind<TInterface>()
        {
            _registrations.Remove(typeof(TInterface));
            return this;
        }

        public IIoCContainer UnBindInstance<TInterface>(TInterface instance = default)
        {
            _registrations.Remove(typeof(TInterface));
            return this;
        }

        public IIoCContainer ClearLifecycle(ELifecycleType lifecycle)
        {
            // 获取所有生命周期类型匹配的注册键
            var keysToRemove = _registrations
                .Where(kv => kv.Value.ELifecycle == lifecycle)
                .Select(kv => kv.Key)
                .ToList();

            // 移除所有匹配的注册项
            foreach (var key in keysToRemove)
            {
                _registrations.Remove(key);
            }

            return this;
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));

        public object Resolve(Type type)
        {
            if(!_registrations.TryGetValue(type, out var registration))
            {
                return null;
            }
            return CreateInstance(registration);
        }

        private object CreateInstance(Registration registration)
        {
            switch(registration.ELifecycle)
            {
                case ELifecycleType.Singleton:
                    if(registration.Instance == null)
                    {
                        registration.Instance = BuildInstance(registration.ImplementationType);
                    }
                    return registration.Instance;
                case ELifecycleType.Transient:
                default:
                    return BuildInstance(registration.ImplementationType);
            }
        }

        private object BuildInstance(Type implementationType)
        {
            // 使用反射创建实例并注入依赖
            var constructor = implementationType.GetConstructors().First();
            var parameters = constructor.GetParameters()
                .Select(p => Resolve(p.ParameterType))
                .ToArray();

            var instance = Activator.CreateInstance(implementationType, parameters);
            InjectDependencies(instance);
            return instance;
        }

        public void InjectDependencies(object instance)
        {
            // 属性注入
            var properties = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(p => p.CanWrite && p.GetCustomAttributes(typeof(InjectAttribute), true).Any());

            foreach (var property in properties)
            {
                var value = Resolve(property.PropertyType);
                property.SetValue(instance, value, null);
            }

            // 字段注入
            var fields = instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.GetCustomAttributes(typeof(InjectAttribute), true).Any());

            // 方法注入
            var methods = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.GetCustomAttributes(typeof(PostConstructAttribute), true).Any());

            foreach (var method in methods)
            {
                var parameters = method.GetParameters()
                    .Select(p => Resolve(p.ParameterType))
                    .ToArray();

                method.Invoke(instance, parameters);
            }
        }
    }

    // 属性注入标记
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {
    }

    // 后构造方法标记
    [AttributeUsage(AttributeTargets.Method)]
    public class PostConstructAttribute : Attribute
    {
    }
}