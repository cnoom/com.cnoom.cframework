using System;
using System.Collections.Generic;

namespace CnoomFrameWork.IoC
{
    /// <summary>
    ///     依赖注入容器核心类，提供类型绑定和实例解析功能
    /// </summary>
    /// <remarks>
    ///     典型用法：
    ///     1. 使用Bind方法配置类型绑定关系
    ///     2. 使用Resolve方法获取实例
    ///     3. 支持单例、瞬态等生命周期配置
    /// </remarks>
    public class DIContainer
    {
        private readonly Dictionary<Type, BindingConfiguration> bindings = new Dictionary<Type, BindingConfiguration>();
        private readonly PropertyInjector propertyInjector;
        private readonly Dictionary<Type, object> singletons = new Dictionary<Type, object>();

        public DIContainer()
        {
            propertyInjector = new PropertyInjector(this);
        }

        /// <summary>
        ///     创建类型绑定配置
        /// </summary>
        /// <typeparam name="TInterface">要绑定的接口或抽象类型</typeparam>
        /// <returns>绑定配置对象，用于进一步设置实现类型和生命周期</returns>
        public BindingConfiguration Bind<TInterface>(Func<TInterface> factory) where TInterface : class
        {
            BindingConfiguration config = new BindingConfiguration(typeof(TInterface), factory);
            bindings[typeof(TInterface)] = config;
            return config;
        }

        public void BindSingleton(object instance)
        {
            Type type = instance.GetType();
            BindingConfiguration config = new BindingConfiguration(type, null);
            bindings[type] = config;
            singletons[type] = instance;
            propertyInjector.Inject(instance);
        }

        /// <summary>
        ///     取消类型绑定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public bool UnBind<T>()
        {
            return UnBind(typeof(T));
        }

        public bool UnBind(Type type)
        {
            if(!bindings.ContainsKey(type)) return false;
            BindingConfiguration config = bindings[type];
            bindings.Remove(type);
            return config.Lifecycle switch
            {
                LifecycleScope.Singleton => singletons.Remove(type),
                LifecycleScope.Transient => true,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        /// <summary>
        ///     解析指定类型的实例
        /// </summary>
        /// <param name="serviceType">要解析的服务类型</param>
        /// <returns>符合生命周期配置的实例</returns>
        /// <exception cref="InvalidOperationException">当找不到类型绑定时抛出</exception>
        /// <exception cref="ArgumentOutOfRangeException">当遇到未定义的生命周期范围时抛出</exception>
        public object Resolve(Type serviceType)
        {
            if(!bindings.TryGetValue(serviceType, out BindingConfiguration config))
            {

                return null;
            }


            return config.Lifecycle switch
            {
                LifecycleScope.Singleton => GetSingletonInstance(config),
                LifecycleScope.Transient => CreateNewInstance(config),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        ///     注入属性
        /// </summary>
        /// <param name="instance"></param>
        public void Inject(object instance)
        {
            propertyInjector.Inject(instance);
        }

        private object GetSingletonInstance(BindingConfiguration config)
        {
            if(singletons.TryGetValue(config.ImplementationType, out object instance)) return instance;
            if(config.Factory == null)
            {
                throw new InvalidOperationException($"No Factory for {config}");
            }
            instance = config.Factory();
            config.AsFactory(null); // 避免重复创建,同时节省内存
            propertyInjector.Inject(instance);
            singletons[config.ImplementationType] = instance;
            return instance;
        }

        private object CreateNewInstance(BindingConfiguration config)
        {
            object instance = config.Factory();
            propertyInjector.Inject(instance);
            return instance;
        }
    }
}