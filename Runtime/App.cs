using CnoomFrameWork.Base.Config;
using CnoomFrameWork.Base.Event;
using CnoomFrameWork.Base.IoC;
using CnoomFrameWork.Base.Log;
using CnoomFrameWork.Singleton;
using UnityEngine;
using UnityEngine.Scripting;

namespace CnoomFrameWork.Core
{
    public class App : PersistentMonoSingleton<App>
    {
        public IEventManager EventManager { get; private set; }
        public ModuleManager ModuleManager { get; private set; }
        public ServiceLocator ServiceLocator { get; private set; }
        public ILog Log { get; private set; }
        internal IIoCContainer IocContainer{get;set;}

        /// <summary>
        ///     全局异常处理回调
        /// </summary>
        /// <param name="condition">异常信息内容</param>
        /// <param name="stackTrace">异常调用堆栈</param>
        /// <param name="type">Unity日志类型</param>
        private void OnHandleException(string condition, string stackTrace, LogType type)
        {
            switch(type)
            {
                case LogType.Log:
                case LogType.Assert:
                    return;
            }
            var message = $"{condition}\n{stackTrace}";
            Log.LogErrorEx($"{nameof(App)} : {message}");
        }

        private App() { }

        /// <summary>
        ///     主入口方法，在场景加载后自动触发
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad), Preserve]
        private static void Main()
        {
            App app = Instance;
            Application.logMessageReceived += app.OnHandleException;
        }

        /// <summary>
        ///     单例初始化时调用，完成核心系统注册
        /// </summary>
        protected override void OnInitialized()
        {
            ConfigManager configManager = ConfigManager.Instance;
            IocContainer = new IoCContainer();
            EventManager = new EventManager();
            Log = configManager.GetConfig<LogConfig>().Log;

            ServiceLocator = new ServiceLocator(IocContainer,EventManager);
            ServiceLocator.AutoRegister();
            
            ModuleManager = new ModuleManager(IocContainer,EventManager);
            ModuleManager.AutoRegisterModule();
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= OnHandleException;
        }
    }
}