using CnoomFrameWork.Base.Config;
using CnoomFrameWork.Base.Log;
using CnoomFrameWork.Singleton;
using CnoomFrameWork.Base.Container;

namespace CnoomFrameWork.Core
{
    public class App : PersistentMonoSingleton<App>
    {
        private App() { }
        public ModuleManager ModuleManager { get; private set; }
        public ServiceLocator ServiceLocator { get; private set; }
        public ILog Log { get; private set; }
        public RootContainer RootContainer { get; private set; }

        /// <summary>
        ///  入口方法
        /// </summary>
        public static void Boot()
        {
            App app = Instance;
        }

        /// <summary>
        ///     单例初始化时调用，完成核心系统注册
        /// </summary>
        protected override void OnInitialized()
        {
            RootContainer = new RootContainer();
            
            ConfigManager configManager = ConfigManager.Instance;

            Log = configManager.GetConfig<LogConfig>().Log;
            RootContainer.BindSingleton<ILog,ILog>(Log);

            ServiceLocator = new ServiceLocator(RootContainer);
            ServiceLocator.AutoRegister();

            ModuleManager = new ModuleManager(RootContainer);
            ModuleManager.AutoRegisterModule();
        }
    }
}