using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Base.Config;
using CnoomFrameWork.Base.IoC;
using CnoomFrameWork.Base.Events;
using UnityEngine.Scripting;

namespace CnoomFrameWork.Core
{
    /// <summary>
    ///     模块管理器
    /// </summary>
    public class ModuleManager
    {
        [Inject, Preserve]
        private IIoCContainer container;

        /// <summary>
        ///     注册模块到DI容器并初始化
        /// </summary>
        /// <param name="module">需要注册的模块实例</param>
        /// <returns>当前模块管理器用于链式调用</returns>
        public ModuleManager RegisterModule<TModule>() where TModule : Module
        {
            RegisterModule<TModule, TModule>();
            return this;
        }

        public ModuleManager RegisterModule<TInterface, TModule>() where TModule : TInterface where TInterface : Module
        {
            container.Bind<TInterface, TModule>(ELifecycleType.Singleton);
            TModule module = container.Resolve<TModule>();
            EventManager.Register(module);
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
            EventManager.Unregister(module);
            module.Dispose();
            return this;
        }

        public ModuleManager UnRegisterModule<T>() where T : Module
        {
            T module = container.Resolve<T>();
            container.UnBindInstance<T>();
            EventManager.Unregister(module);
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
            return container.Resolve<T>();
        }

        internal void AutoRegisterModule()
        {
            ModuleOrderConfig config = ConfigManager.Instance.GetConfig<ModuleOrderConfig>();
            foreach (IIocRegister handler in config.Registers)
            {
                Module module = handler.Register(container) as Module;
                EventManager.Register(module);
                module.Initialize();
            }
        }
    }
}