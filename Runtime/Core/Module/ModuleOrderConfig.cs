using System;
using System.Collections.Generic;
using CnoomFrameWork.Modules.ActionModule;
using CnoomFrameWork.Modules.AddressableModule;
using CnoomFrameWork.Modules.UiModule;
using Modules.ComponentContainerModule;

namespace CnoomFrameWork.Core
{
    /// <summary>
    ///     自动模块排序配置类，用于管理模块的加载顺序。
    ///     若未模块未配置则按默认顺序加载
    /// </summary>
    public class ModuleOrderConfig : IConfig
    {
        public Dictionary<Type, int> ModuleOrders { get; } = new Dictionary<Type, int>();

        public ModuleOrderConfig()
        {
            AddModule<ActionManager>(1000);
            AddModule<GoContainerModule>(990);
            AddModule<AssetsModule>(950);
            AddModule<UIModule>(900);
        }
        
        protected void AddModule<T>(int order) where T : Module
        {
            ModuleOrders.Add(typeof(T), order);
        }
    }
}