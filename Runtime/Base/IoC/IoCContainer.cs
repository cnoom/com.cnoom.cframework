using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CnoomFrameWork.Base.IoC
{
    public class IoCContainer : IIoCContainer
    {

        protected readonly Dictionary<Type, Registration> Registrations = new Dictionary<Type, Registration>();

        public void Bind<TInterface, TImplementation>(ELifecycleType eLifecycle = ELifecycleType.Transient)
            where TImplementation : TInterface
        {
            Bind<TInterface>(typeof(TImplementation), eLifecycle);
        }

        public void Bind<TInterface>(Type implementationType, ELifecycleType eLifecycle = ELifecycleType.Transient)
        {
            Type interfaceType = typeof(TInterface);
            Registrations[interfaceType] = new Registration
            {
                ImplementationType = implementationType,
                ELifecycle = eLifecycle
            };
        }

        public void BindInstance<TInterface>(TInterface instance)
        {
            Type interfaceType = typeof(TInterface);
            Registrations[interfaceType] = new Registration
            {
                Instance = instance,
                ELifecycle = ELifecycleType.Singleton
            };
        }

        public void UnBind<TInterface>()
        {
            Registrations.Remove(typeof(TInterface));
        }

        public void UnBindInstance<TInterface>(TInterface instance = default)
        {
            Registrations.Remove(typeof(TInterface));
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public virtual object Resolve(Type type)
        {
            if(!Registrations.TryGetValue(type, out Registration registration))
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

        protected object CreateInstance(Registration registration)
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
        
        protected class Registration
        {
            public Type ImplementationType { get; set; }
            public object Instance { get; set; }
            public ELifecycleType ELifecycle { get; set; }
        }
    }
}