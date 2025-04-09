using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CnoomFrameWork.Base.IoC
{
    public class IoCContainer : IIoCContainer
    {

        private readonly Dictionary<Type, Registration> _registrations = new Dictionary<Type, Registration>();

        public IIoCContainer Bind<TInterface, TImplementation>(ELifecycleType eLifecycle = ELifecycleType.Transient)
            where TImplementation : TInterface
        {
            return Bind<TInterface>(typeof(TImplementation), eLifecycle);
        }

        public IIoCContainer Bind<TInterface>(Type implementationType, ELifecycleType eLifecycle = ELifecycleType.Transient)
        {
            Type interfaceType = typeof(TInterface);
            _registrations[interfaceType] = new Registration
            {
                ImplementationType = implementationType,
                ELifecycle = eLifecycle
            };
            return this;
        }

        public IIoCContainer BindInstance<TInterface>(TInterface instance)
        {
            Type interfaceType = typeof(TInterface);
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
            List<Type> keysToRemove = _registrations
                .Where(kv => kv.Value.ELifecycle == lifecycle)
                .Select(kv => kv.Key)
                .ToList();

            // 移除所有匹配的注册项
            foreach (Type key in keysToRemove)
            {
                _registrations.Remove(key);
            }

            return this;
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            if(!_registrations.TryGetValue(type, out Registration registration))
            {
                return null;
            }
            return CreateInstance(registration);
        }

        public void InjectDependencies(object instance)
        {
            // 属性注入
            IEnumerable<PropertyInfo> properties = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(p => p.CanWrite && p.GetCustomAttributes(typeof(InjectAttribute), true).Any());

            foreach (PropertyInfo property in properties)
            {
                object value = Resolve(property.PropertyType);
                property.SetValue(instance, value, null);
            }

            // 字段注入
            IEnumerable<FieldInfo> fields = instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.GetCustomAttributes(typeof(InjectAttribute), true).Any());

            foreach (FieldInfo field in fields)
            {
                object value = Resolve(field.FieldType);
                field.SetValue(instance, value);
            }

            // 方法注入
            IEnumerable<MethodInfo> methods = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.GetCustomAttributes(typeof(PostConstructAttribute), true).Any());

            foreach (MethodInfo method in methods)
            {
                object[] parameters = method.GetParameters()
                    .Select(p => Resolve(p.ParameterType))
                    .ToArray();

                method.Invoke(instance, parameters);
            }
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
            ConstructorInfo constructor = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).First();
            object[] parameters = constructor.GetParameters()
                .Select(p => Resolve(p.ParameterType))
                .ToArray();

            object instance = Activator.CreateInstance(implementationType, parameters);
            InjectDependencies(instance);
            return instance;
        }
        private class Registration
        {
            public Type ImplementationType { get; set; }
            public object Instance { get; set; }
            public ELifecycleType ELifecycle { get; set; }
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