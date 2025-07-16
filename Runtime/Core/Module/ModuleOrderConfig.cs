using CnoomFrameWork.Base.Container;
using CnoomFrameWork.Modules.UiModule;

namespace CnoomFrameWork.Core
{
    /// <summary>
    ///     自动模块排序配置类，用于管理模块的加载顺序。
    ///     模块的加载顺序由ModuleOrderConfig中的ModuleOrders字典决定。
    ///     order大的模块优先注册,默认order为零。
    /// </summary>
    public class ModuleOrderConfig : IocRegisterConfig
    {
        public ModuleOrderConfig()
        {
            Register<UIModule>(960);
        }
    }
}