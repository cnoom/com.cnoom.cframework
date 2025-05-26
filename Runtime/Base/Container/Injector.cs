using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CnoomFrameWork.Core;

namespace CnoomFrameWork.Base.Container
{
    public static class Injector
    {
        
        public static void Inject(object instance)
        {
            RootContainer container = App.Instance.RootContainer;
            // 属性注入
            IEnumerable<PropertyInfo> properties = instance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(p => p.CanWrite && p.GetCustomAttributes(typeof(InjectAttribute), true).Any());

            foreach (PropertyInfo property in properties)
            {
                InjectAttribute attribute =
                    property.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault() as InjectAttribute;
                object value = GetContainer(attribute.ContainerName, container).Resolve(property.PropertyType);
                property.SetValue(instance, value, null);
            }

            // 字段注入
            IEnumerable<FieldInfo> fields = instance.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.GetCustomAttributes(typeof(InjectAttribute), true).Any());

            foreach (FieldInfo field in fields)
            {
                InjectAttribute attribute =
                    field.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault() as InjectAttribute;
                object value = GetContainer(attribute.ContainerName, container).Resolve(field.FieldType);
                field.SetValue(instance, value);
            }

            // 方法注入
            IEnumerable<MethodInfo> methods = instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.GetCustomAttributes(typeof(PostConstructAttribute), true).Any());

            foreach (MethodInfo method in methods)
            {
                InjectAttribute attribute =
                    method.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault() as InjectAttribute;
                object[] parameters = method.GetParameters()
                    .Select(p => GetContainer(attribute.ContainerName, container).Resolve(p.ParameterType))
                    .ToArray();

                method.Invoke(instance, parameters);
            }
        }

        private static RootContainer GetContainer(string containerName, RootContainer rootContainer)
        {
            if (string.IsNullOrEmpty(containerName)) return rootContainer;
            string[] paths = containerName.Split('/');
            RootContainer currentContainer = rootContainer;
            foreach (string path in paths)
            {
                currentContainer = currentContainer.GetChildContainer(path);
            }

            return currentContainer;
        }
    }
}