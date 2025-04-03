using CnoomFrameWork.Base.Log;

namespace CnoomFrameWork.Core
{
    public static class AppExtensions
    {
        public static void LogTest(this App app, string message)
        {
            app.Log.ColorLogTestEx(nameof(App), message);
        }

        public static void Inject(this App app,object o)
        {
            app.IocContainer.InjectDependencies(o);
        }

        public static void RegisterModule<TModule>(this App app) where TModule : Module
        {
            app.ModuleManager.RegisterModule<TModule>();
        }

        public static void RegisterModule<TInterface, TImplementation>(this App app) where TInterface : Module
            where TImplementation : TInterface
        {
            app.ModuleManager.RegisterModule<TInterface, TImplementation>();
        }

        public static void UnRegisterModule<TModule>(this App app) where TModule : Module
        {
            app.ModuleManager.UnRegisterModule<TModule>();
        }

        public static void RegisterSubscriber(this App app, object subscriber)
        {
            app.EventManager.AutoUnregister(subscriber);
        }

        public static void RegisterService<TInterface, TService>(this App app) where TInterface : class, IService where TService : TInterface
        {
            app.ServiceLocator.RegisterService<TInterface, TService>();
        }

        public static void RegisterService<TInterface>(this App app) where TInterface : class, IService
        {
            app.ServiceLocator.UnRegisterService<TInterface>();
        }

        public static void UnRegisterSubscriber(this App app, object subscriber)
        {
            app.EventManager.AutoUnregister(subscriber);
        }

        public static void Publish<TEvent>(this App app, TEvent evt)
        {
            app.EventManager.Publish(evt);
        }
    }
}