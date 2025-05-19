using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CnoomFrameWork.Base.IoC
{
    public static class Injector
    {
        public static void Inject(object instance,IIoCContainer cContainer)
        {
            // 属性注入
            IEnumerable<PropertyInfo> properties = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(p => p.CanWrite && p.GetCustomAttributes(typeof(InjectAttribute), true).Any());

            foreach (PropertyInfo property in properties)
            {
                object value = cContainer.Resolve(property.PropertyType);
                property.SetValue(instance, value, null);
            }

            // 字段注入
            IEnumerable<FieldInfo> fields = instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.GetCustomAttributes(typeof(InjectAttribute), true).Any());

            foreach (FieldInfo field in fields)
            {
                object value = cContainer.Resolve(field.FieldType);
                field.SetValue(instance, value);
            }

            // 方法注入
            IEnumerable<MethodInfo> methods = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.GetCustomAttributes(typeof(PostConstructAttribute), true).Any());

            foreach (MethodInfo method in methods)
            {
                object[] parameters = method.GetParameters()
                    .Select(p => cContainer.Resolve(p.ParameterType))
                    .ToArray();

                method.Invoke(instance, parameters);
            }
        }
    }
}