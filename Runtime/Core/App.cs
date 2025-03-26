using System;
using CnoomFrameWork.Event;
using CnoomFrameWork.IoC;
using CnoomFrameWork.Log;
using CnoomFrameWork.Singleton;
using UnityEngine;
using UnityEngine.Scripting;

namespace CnoomFrameWork.Core
{
    public class App : PersistentMonoSingleton<App>
    {
        private EventBus eventBus;
        private ModuleManager moduleManager;
        public DIContainer Container { get; private set; }

        public ILog Log { get; private set; }

        #region IoC

        public void Inject(object obj)
        {
            Container.Inject(obj);
        }

        #endregion

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

        #region 初始化

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
            Container = new DIContainer();
            Container.BindSingleton(Container);

            Log = ConfigManager.Instance.GetConfig<LogConfig>().Log;

            Container.Bind(() => Log).In(LifecycleScope.Singleton);
            Container.BindSingleton(new EventBus());
            Container.BindSingleton(new ModuleManager());

            eventBus = Container.Resolve<EventBus>();
            moduleManager = Container.Resolve<ModuleManager>();
            moduleManager.AutoRegisterModule();
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= OnHandleException;
        }

        #endregion

        #region 模块管理

        /// <summary>
        ///     注册自定义模块到应用系统
        /// </summary>
        /// <param name="module">需要注册的模块实例</param>
        /// <returns>模块管理器实例用于链式调用</returns>
        public ModuleManager RegisterModule(Module module)
        {
            return moduleManager.RegisterModule(module);
        }

        /// <summary>
        ///     从应用系统注销模块
        /// </summary>
        /// <param name="module">需要注销的模块实例</param>
        /// <returns>模块管理器实例用于链式调用</returns>
        public ModuleManager UnRegisterModule(Module module)
        {
            return moduleManager.UnRegisterModule(module);
        }

        #endregion

        #region 事件系统

        public void AutoSubscribe(object obj)
        {
            eventBus.AutoRegister(obj);
        }

        public void AutoUnsubscribe(object obj)
        {
            eventBus.AutoUnregister(obj);
        }

        public void Subscribe<T>(Action<T> action)
        {
            eventBus.Subscribe(action);
        }

        public void UnSubscribe<T>(Action<T> action)
        {
            eventBus.Unsubscribe(action);
        }

        public void Publish<T>(T message)
        {
            eventBus.Publish(message);
        }

        #endregion
    }
}