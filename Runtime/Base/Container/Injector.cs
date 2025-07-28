using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CnoomFrameWork.Core;

namespace CnoomFrameWork.Base.Container
{
    public static class Injector
    {
        // 缓存类型的注入信息，避免重复反射
        private static readonly Dictionary<Type, InjectionInfo> InjectionCache = new Dictionary<Type, InjectionInfo>();
        
        // 用于线程安全的锁对象
        private static readonly object CacheLock = new object();

        public static void Inject(object instance)
        {
            if (instance == null) return;
            
            var container = App.Instance.RootContainer;
            var type = instance.GetType();
            
            // 从缓存获取或创建注入信息
            var injectionInfo = GetInjectionInfo(type);
            
            // 属性注入
            foreach (var propertyInfo in injectionInfo.Properties)
            {
                var value = GetContainer(propertyInfo.ContainerName, container).Resolve(propertyInfo.Type);
                propertyInfo.Property.SetValue(instance, value, null);
            }

            // 字段注入
            foreach (var fieldInfo in injectionInfo.Fields)
            {
                var value = GetContainer(fieldInfo.ContainerName, container).Resolve(fieldInfo.Type);
                fieldInfo.Field.SetValue(instance, value);
            }

            // 方法注入
            foreach (var methodInfo in injectionInfo.Methods)
            {
                var parameters = methodInfo.ParameterTypes
                    .Select(p => GetContainer(methodInfo.ContainerName, container).Resolve(p))
                    .ToArray();

                methodInfo.Method.Invoke(instance, parameters);
            }
        }
        
        // 获取类型的注入信息，如果缓存中没有则创建并缓存
        private static InjectionInfo GetInjectionInfo(Type type)
        {
            lock (CacheLock)
            {
                if (InjectionCache.TryGetValue(type, out var info))
                {
                    return info;
                }
                
                info = new InjectionInfo();
                
                // 查找需要注入的属性
                var properties = type
                    .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(p => p.CanWrite && p.GetCustomAttributes(typeof(InjectAttribute), true).Any());

                foreach (var property in properties)
                {
                    var attribute = property.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault() as InjectAttribute;
                    info.Properties.Add(new PropertyInjectionInfo
                    {
                        Property = property,
                        Type = property.PropertyType,
                        ContainerName = attribute?.ContainerName ?? string.Empty
                    });
                }

                // 查找需要注入的字段
                var fields = type
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(f => f.GetCustomAttributes(typeof(InjectAttribute), true).Any());

                foreach (var field in fields)
                {
                    var attribute = field.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault() as InjectAttribute;
                    info.Fields.Add(new FieldInjectionInfo
                    {
                        Field = field,
                        Type = field.FieldType,
                        ContainerName = attribute?.ContainerName ?? string.Empty
                    });
                }

                // 查找需要注入的方法
                var methods = type
                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(m => m.GetCustomAttributes(typeof(PostConstructAttribute), true).Any());

                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault() as InjectAttribute;
                    info.Methods.Add(new MethodInjectionInfo
                    {
                        Method = method,
                        ParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray(),
                        ContainerName = attribute?.ContainerName ?? string.Empty
                    });
                }
                
                InjectionCache[type] = info;
                return info;
            }
        }

        private static RootContainer GetContainer(string containerName, RootContainer rootContainer)
        {
            if (string.IsNullOrEmpty(containerName)) return rootContainer;
            var paths = containerName.Split('/');
            RootContainer currentContainer = rootContainer;
            foreach (var path in paths) currentContainer = currentContainer.GetChildContainer(path);

            return currentContainer;
        }
        
        // 存储类型的注入信息的类
        private class InjectionInfo
        {
            public List<PropertyInjectionInfo> Properties { get; } = new List<PropertyInjectionInfo>();
            public List<FieldInjectionInfo> Fields { get; } = new List<FieldInjectionInfo>();
            public List<MethodInjectionInfo> Methods { get; } = new List<MethodInjectionInfo>();
        }
        
        private class PropertyInjectionInfo
        {
            public PropertyInfo Property { get; set; }
            public Type Type { get; set; }
            public string ContainerName { get; set; }
        }
        
        private class FieldInjectionInfo
        {
            public FieldInfo Field { get; set; }
            public Type Type { get; set; }
            public string ContainerName { get; set; }
        }
        
        private class MethodInjectionInfo
        {
            public MethodInfo Method { get; set; }
            public Type[] ParameterTypes { get; set; }
            public string ContainerName { get; set; }
        }
    }
}