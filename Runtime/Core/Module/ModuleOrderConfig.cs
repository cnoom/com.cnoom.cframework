using System;
using System.Collections.Generic;
using CnoomFrameWork.Modules.ActionModule;
using CnoomFrameWork.Modules.AddressableModule;
using CnoomFrameWork.Modules.UiModule;
using Modules.ComponentContainerModule;
using Modules.StorageModule;

namespace CnoomFrameWork.Core
{
    /// <summary>
    ///     自动模块排序配置类，用于管理模块的加载顺序。
    ///     模块的加载顺序由ModuleOrderConfig中的ModuleOrders字典决定。
    ///     order大的模块优先注册,默认order为零。
    /// </summary>
    public class ModuleOrderConfig : IConfig
    {
        internal Dictionary<Type, int> ModuleOrders { get; } = new Dictionary<Type, int>();

        public ModuleOrderConfig()
        {
            AddModule<ActionManager>(1000);
            AddModule<StorageModule>(990);
            AddModule<GoContainerModule>(980);
            AddModule<AssetsModule>(970);
            AddModule<UIModule>(960);
        }
        
        public void AddModule<T>(int order = 0) where T : Module
        {
            ModuleOrders.Add(typeof(T), order);
        }
    }
}