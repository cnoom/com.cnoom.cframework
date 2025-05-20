using System;
using System.Linq;
using System.Reflection;

namespace CnoomFrameWork.Base.Container
{
    public static class InstanceFactory
    {
        public static T CreateInstance<T>(BaseContainer container)
        {
            return (T)Create(typeof(T), container);
        }
        
        private static object Create(Type implementationType, BaseContainer container)
        {
            // 使用反射创建实例并注入依赖
            ConstructorInfo constructor = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).First();
            object[] parameters = constructor.GetParameters()
                .Select(p => container.Resolve(p.ParameterType))
                .ToArray();

            object instance = Activator.CreateInstance(implementationType, parameters);
            Injector.Inject(instance,container);
            return instance;
        }
    }
}