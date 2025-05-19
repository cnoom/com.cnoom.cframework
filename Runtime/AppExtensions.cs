using System;
using CnoomFrameWork.Base.IoC;
using CnoomFrameWork.Base.Log;
using CnoomFrameWork.Core.Base.DelayManager;
using UnityEngine;

namespace CnoomFrameWork.Core
{
    public static class AppExtensions
    {

        #region Log

        public static void LogTest(this App app, string message, string sender = "")
        {
            app.Log.ColorLogTestEx(sender, message);
        }

        public static void Log(this App app, string message, string sender = "")
        {
            app.Log.ColorLogEx(sender, message);
        }

        public static void LogWarning(this App app, string message, string sender = "")
        {
            app.Log.ColorLogWarningEx(sender, message);
        }

        public static void LogError(this App app, string message, string sender = "")
        {
            app.Log.ColorLogErrorEx(sender, message);
        }

        #endregion

        #region IoCContainer

        public static void Inject(this App app, object o)
        {
            Injector.Inject(o,app.IocContainer);
        }

        public static void BindInstance<TInterface>(this App app, TInterface instance)
        {
            app.IocContainer.BindInstance(instance);
        }

        public static void UnBindInstance<TInterface>(this App app)
        {
            app.IocContainer.UnBindInstance<TInterface>();
        }

        public static T Resolve<T>(this App app)
        {
            return app.IocContainer.Resolve<T>();
        }

        #endregion

        #region ModuleManager

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
        
        public static T GetModule<T>(this App app) where T : Module
        {
            return app.ModuleManager.GetModule<T>();
        }

        #endregion

        #region ServiceManager

        public static void RegisterService<TInterface, TService>(this App app) where TInterface : class, IService where TService : TInterface
        {
            app.ServiceLocator.RegisterService<TInterface, TService>();
        }

        public static void RegisterService<TInterface>(this App app) where TInterface : class, IService
        {
            app.ServiceLocator.RegisterService<TInterface, TInterface>();
        }

        public static void UnRegisterService<TInterface>(this App app) where TInterface : class, IService
        {
            app.ServiceLocator.UnRegisterService<TInterface>();
        }

        public static TInterface GetService<TInterface>(this App app) where TInterface : class, IService
        {
            return app.ServiceLocator.GetService<TInterface>();
        }

        #endregion

        #region Delay

        public static DelayManager.CancellationToken Delay(this App app, float delay, Action action, GameObject bindGameObject = null)
        {
            return DelayManager.Instance.RegisterTimeDelay(delay, action, bindGameObject);
        }

        public static DelayManager.CancellationToken Delay(this App app, int frame, Action action, GameObject bindGameObject = null)
        {
            return DelayManager.Instance.RegisterFrameDelay(frame, action, bindGameObject);
        }

        public static void Cancel(this App app, DelayManager.CancellationToken token)
        {
            DelayManager.Instance.CancelDelay(token);
        }

        #endregion
    }
}