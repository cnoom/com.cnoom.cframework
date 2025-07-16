using System.Linq;
using System.Reflection;
using CnoomFrameWork.Core;

namespace CnoomFrameWork.Base.Container
{
    public static class Injector
    {
        public static void Inject(object instance)
        {
            var container = App.Instance.RootContainer;
            // 属性注入
            var properties = instance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(p => p.CanWrite && p.GetCustomAttributes(typeof(InjectAttribute), true).Any());

            foreach (var property in properties)
            {
                var attribute =
                    property.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault() as InjectAttribute;
                var value = GetContainer(attribute.ContainerName, container).Resolve(property.PropertyType);
                property.SetValue(instance, value, null);
            }

            // 字段注入
            var fields = instance.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.GetCustomAttributes(typeof(InjectAttribute), true).Any());

            foreach (var field in fields)
            {
                var attribute =
                    field.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault() as InjectAttribute;
                var value = GetContainer(attribute.ContainerName, container).Resolve(field.FieldType);
                field.SetValue(instance, value);
            }

            // 方法注入
            var methods = instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.GetCustomAttributes(typeof(PostConstructAttribute), true).Any());

            foreach (var method in methods)
            {
                var attribute =
                    method.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault() as InjectAttribute;
                var parameters = method.GetParameters()
                    .Select(p => GetContainer(attribute.ContainerName, container).Resolve(p.ParameterType))
                    .ToArray();

                method.Invoke(instance, parameters);
            }
        }

        private static RootContainer GetContainer(string containerName, RootContainer rootContainer)
        {
            if (string.IsNullOrEmpty(containerName)) return rootContainer;
            var paths = containerName.Split('/');
            var currentContainer = rootContainer;
            foreach (var path in paths) currentContainer = currentContainer.GetChildContainer(path);

            return currentContainer;
        }
    }
}