using System;
using System.Collections.Generic;
using CnoomFrameWork.Modules.ActionModule;
using CnoomFrameWork.Modules.AddressableModule;
using CnoomFrameWork.Modules.UiModule;

namespace CnoomFrameWork.Core
{
    /// <summary>
    ///     自动模块排序配置类，用于管理模块的加载顺序。
    ///     若未模块未配置则按默认顺序加载
    /// </summary>
    public class ModuleOrderConfig : IConfig
    {
        public Dictionary<Type, int> ModuleOrders { get; private set; } = new Dictionary<Type, int>
        {
            {
                typeof(ActionManager), 1000
            },
            {
                typeof(AssetsModule), 950
            },
            {
                typeof(UIModule), 900
            }
        };
    }
}