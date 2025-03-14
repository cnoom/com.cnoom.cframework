using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CnoomFrameWork.Event;
using CnoomFrameWork.IoC;
using CnoomFrameWork.Log;

namespace CnoomFrameWork.Core
{
    /// <summary>
    ///     模块管理器，后续在考虑实现初始模组替换功能
    /// </summary>
    public class ModuleManager
    {
        private readonly List<Module> modules = new List<Module>();
        internal ModuleManager()
        {
        }
        [Inject] private EventBus EventBus { get; set; }
        [Inject] private ILog Log { get; set; }
        [Inject] private DIContainer DiContainer { get; set; }

        /// <summary>
        ///     注册模块到DI容器并初始化
        /// </summary>
        /// <param name="module">需要注册的模块实例</param>
        /// <returns>当前模块管理器用于链式调用</returns>
        public ModuleManager RegisterModule(Module module)
        {
            DiContainer.BindSingleton(module);
            modules.Add(module);
            module.Initialize();
            return this;
        }

        /// <summary>
        ///     从管理器移除模块并释放资源
        /// </summary>
        /// <param name="module">需要移除的模块实例</param>
        /// <returns>当前模块管理器用于链式调用</returns>
        public ModuleManager UnRegisterModule(Module module)
        {
            DiContainer.UnBind(module.GetType());
            modules.Remove(module);
            module.Dispose();
            return this;
        }

        /// <summary>
        ///     获取指定类型的模块实例
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>找到的模块实例，未找到时返回null</returns>
        public T GetModule<T>() where T : Module
        {
            return modules.OfType<T>().FirstOrDefault();
        }

        internal void AutoRegisterModule()
        {
            Type moduleType = typeof(Module);
            List<Tuple<int, Type>> list = new List<Tuple<int, Type>>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (TypeInfo type in assembly.DefinedTypes)
                {
                    if(type.IsAbstract) continue;
                    if(!moduleType.IsAssignableFrom(type.AsType())) continue;
                    AutoRegisterModuleAttribute attribute = type.GetCustomAttribute<AutoRegisterModuleAttribute>();
                    if(attribute == null) continue;
                    list.Add(new Tuple<int, Type>(attribute.Order, type.AsType()));
                }
            }
            list.Sort((x, y) => y.Item1.CompareTo(x.Item1));
            foreach ((int _, Type type) in list)
            {
                Module module = (Module)Activator.CreateInstance(type);
                RegisterModule(module);
            }
        }
    }
}