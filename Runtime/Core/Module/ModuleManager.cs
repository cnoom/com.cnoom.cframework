using CnoomFrameWork.Base.Config;
using CnoomFrameWork.Base.Events;
using CnoomFrameWork.Base.Container;

namespace CnoomFrameWork.Core
{
    /// <summary>
    ///     模块管理器
    /// </summary>
    public class ModuleManager
    {
        
        private RootContainer _rootContainer;

        internal ModuleManager(RootContainer rootContainer)
        {
            _rootContainer = rootContainer;
        }

        /// <summary>
        /// 注册模块到依赖注入容器并完成初始化。
        /// 模块实例通过工厂创建，并绑定为单例服务，同时注册为事件处理器。
        /// </summary>
        /// <typeparam name="TModule">模块的具体实现类型，必须继承自 Module</typeparam>
        public void RegisterModule<TModule>() where TModule : Module
        {
            RegisterModule<TModule, TModule>();
        }

        /// <summary>
        /// 注册模块到依赖注入容器并完成初始化。
        /// 模块实例通过工厂创建，并绑定为单例服务，同时注册为事件处理器。
        /// </summary>
        /// <typeparam name="TInterface">模块的接口类型，必须继承自 Module</typeparam>
        /// <typeparam name="TModule">模块的具体实现类型，必须实现 TInterface</typeparam>
        public void RegisterModule<TInterface, TModule>() where TModule : TInterface where TInterface : Module
        {
            TModule module = InstanceFactory.CreateInstance<TModule>(_rootContainer);
            _rootContainer.BindSingleton<TInterface,TModule>(module);
            EventManager.Register(module);
            module.Initialize();
        }

        /// <summary>
        /// 从DI容器中注销指定模块，并清理相关资源
        /// </summary>
        /// <typeparam name="TInterface">要注销的模块类型，必须实现 Module 接口</typeparam>
        /// <remarks>
        /// 该方法会执行以下操作：
        /// 1. 从 RootContainer 中解析指定类型的模块实例。
        /// 2. 解绑该模块在 RootContainer 中的单例注册。
        /// 3. 从事件管理系统中注销该模块的所有事件处理器。
        /// 4. 调用模块的 Dispose 方法以释放其占用的资源。
        /// </remarks>
        public void UnRegisterModule<TInterface>() where TInterface : Module
        {
            TInterface module = _rootContainer.Resolve<TInterface>();
            _rootContainer.UnBindSingleton<TInterface>();
            EventManager.Unregister(module);
            module.Dispose();
        }

        /// <summary>
        ///     获取指定类型的模块实例
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>找到的模块实例，未找到时返回null</returns>
        public T GetModule<T>() where T : Module
        {
            return _rootContainer.Resolve<T>();
        }

        
        internal void AutoRegisterModule()
        {
            ModuleOrderConfig config = ConfigManager.Instance.GetConfig<ModuleOrderConfig>();
            foreach (IIocRegister handler in config.Registers)
            {
                Module module = handler.Register(_rootContainer) as Module;
                EventManager.Register(module);
                module.Initialize();
            }
        }
    }
}