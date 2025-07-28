using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CnoomFrameWork.Base.Container
{
    public static class InstanceFactory
    {
        // 缓存类型的构造函数信息
        private static readonly Dictionary<Type, ConstructorInfo> ConstructorCache = new Dictionary<Type, ConstructorInfo>();
        
        // 用于线程安全的锁对象
        private static readonly object CacheLock = new object();

        public static T CreateInstance<T>(BaseContainer container)
        {
            return (T)Create(typeof(T), container);
        }

        private static object Create(Type implementationType, BaseContainer container)
        {
            // 从缓存获取或查找最合适的构造函数
            var constructor = GetBestConstructor(implementationType);
            
            // 解析构造函数参数
            var parameters = constructor.GetParameters()
                .Select(p => container.Resolve(p.ParameterType))
                .ToArray();

            // 创建实例并注入依赖
            var instance = Activator.CreateInstance(implementationType, parameters);
            Injector.Inject(instance);
            return instance;
        }
        
        // 获取最合适的构造函数（优先选择参数最少的公共构造函数）
        private static ConstructorInfo GetBestConstructor(Type type)
        {
            lock (CacheLock)
            {
                if (ConstructorCache.TryGetValue(type, out var cachedConstructor))
                {
                    return cachedConstructor;
                }
                
                var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                
                // 如果没有构造函数，抛出异常
                if (constructors.Length == 0)
                {
                    throw new InvalidOperationException($"类型 {type.FullName} 没有可用的构造函数");
                }
                
                // 优先选择公共构造函数，其次按参数数量排序
                var bestConstructor = constructors
                    .OrderByDescending(c => c.IsPublic)
                    .ThenBy(c => c.GetParameters().Length)
                    .First();
                
                ConstructorCache[type] = bestConstructor;
                return bestConstructor;
            }
        }
    }
}
