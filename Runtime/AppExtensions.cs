using System;
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
            app.IocContainer.InjectDependencies(o);
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

        #endregion

        #region ServiceManager

        public static void RegisterService<TInterface, TService>(this App app) where TInterface : class, IService where TService : TInterface
        {
            app.ServiceLocator.RegisterService<TInterface, TService>();
        }

        public static void RegisterService<TInterface>(this App app) where TInterface : class, IService
        {
            app.ServiceLocator.RegisterService<TInterface,TInterface>();
        }

        public static void UnRegisterService<TInterface>(this App app) where TInterface : class, IService
        {
            app.ServiceLocator.UnRegisterService<TInterface>();
        }

        #endregion

        #region EventManager

        public static void UnRegisterSubscriber(this App app, object subscriber)
        {
            app.EventManager.AutoUnSubscribe(subscriber);
        }

        public static void RegisterSubscriber(this App app, object subscriber)
        {
            app.EventManager.AutoUnSubscribe(subscriber);
        }

        public static void AutoSubscribe(this App app, object subscriber)
        {
            app.EventManager.AutoSubscribe(subscriber);
        }

        public static void AutoUnSubscribe(this App app, object subscriber)
        {
            app.EventManager.AutoUnSubscribe(subscriber);
        }

        public static void Publish<TEvent>(this App app, TEvent evt)
        {
            app.EventManager.Publish(evt);
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