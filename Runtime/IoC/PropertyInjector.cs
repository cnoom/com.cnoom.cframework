using System;
using System.Reflection;

namespace CnoomFrameWork.IoC
{
    /// <summary>
    ///     属性注入器，负责对已创建实例进行属性级别依赖注入
    /// </summary>
    /// <remarks>
    ///     仅注入标记[Inject]特性且具有set访问器的公共属性
    /// </remarks>
    internal class PropertyInjector
    {
        private readonly DIContainer container;

        public PropertyInjector(DIContainer container)
        {
            this.container = container;
        }

        /// <summary>
        ///     执行属性注入操作
        /// </summary>
        /// <param name="instance">需要注入属性的实例</param>
        /// <exception cref="InvalidOperationException">当依赖解析失败时抛出</exception>
        public void Inject(object instance)
        {
            Type type = instance.GetType();
            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                InjectAttribute injectAttribute = prop.GetCustomAttribute<InjectAttribute>();
                if(injectAttribute == null || !prop.CanWrite) continue;
                Type propType = prop.PropertyType;
                object propValue = container.Resolve(propType);
                prop.SetValue(instance, propValue);
            }
        }
    }

    /// <summary>
    ///     标记需要依赖注入的属性
    /// </summary>
    /// <remarks>
    ///     该特性仅对可写(writable)的实例属性有效
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
    }
}