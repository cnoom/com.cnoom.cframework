using System;
using System.Runtime.CompilerServices;

namespace CnoomFrameWork.Base.Events
{
    /// <summary>
    ///     事件管理器，提供全局事件系统访问点
    /// </summary>
    public static class EventManager
    {
        // 使用静态只读字段提高性能
        private static readonly EventHandler Handler = new();
        private static readonly RefEventHandlers RefHandler = new();
        private static readonly CallbackEventHandler CallbackHandler = new();

        /// <summary>
        ///     注册订阅者，自动扫描特性标记的方法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register(object subscriber)
        {
            Handler.Register(subscriber);
            RefHandler.Register(subscriber);
            CallbackHandler.Register(subscriber);
        }

        /// <summary>
        ///     注销订阅者，移除所有相关的事件处理器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unregister(object subscriber)
        {
            Handler.Unregister(subscriber);
            RefHandler.Unregister(subscriber);
            CallbackHandler.Unregister(subscriber);
        }

        #region 回调事件系统

        /// <summary>
        ///     订阅回调事件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubscribeCallback<T, TResult>(CallbackEventHandler.CallbackEvent<T, TResult> handler,
            int priority = 0, bool once = false)
        {
            CallbackHandler.Subscribe(handler, priority, once);
        }

        /// <summary>
        ///     取消订阅回调事件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnsubscribeCallback<T, TResult>(CallbackEventHandler.CallbackEvent<T, TResult> handler)
        {
            CallbackHandler.Unsubscribe(handler);
        }

        /// <summary>
        ///     发布回调事件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PublishCallback<T, TResult>(T e, Action<TResult> callback)
        {
            CallbackHandler.Publish(e, callback);
        }
        
        #endregion

        #region 普通事件系统

        /// <summary>
        ///     订阅普通事件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Subscribe<T>(Action<T> handler, int priority = 0, bool once = false)
        {
            Handler.Subscribe(handler, priority, once);
        }

        /// <summary>
        ///     取消订阅普通事件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unsubscribe<T>(Action<T> handler)
        {
            Handler.Unsubscribe(handler);
        }

        /// <summary>
        ///     发布普通事件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Publish<T>(T e)
        {
            Handler.Publish(e);
        }
        
        /// <summary>
        ///     获取可重用的事件对象
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetEventObject<T>() where T : class, new()
        {
            return Handler.GetEventObject<T>();
        }

        /// <summary>
        ///     释放事件对象回对象池
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReleaseEventObject<T>(T eventObj) where T : class
        {
            Handler.ReleaseEventObject(eventObj);
        }
        
        /// <summary>
        ///     添加普通事件过滤器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFilter<T>(Func<T, Delegate, bool> filter)
        {
            Handler.AddFilter(filter);
        }

        #endregion

        #region 引用结构体事件系统

        /// <summary>
        ///     订阅结构体事件（ref传参）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubscribeRef<T>(RefEventHandlers.RefEvent<T> handler, int priority = 0,
            bool once = false)
            where T : struct
        {
            RefHandler.Subscribe(handler, priority, once);
        }

        /// <summary>
        ///     取消订阅结构体事件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnSubscribeRef<T>(RefEventHandlers.RefEvent<T> handler) where T : struct
        {
            RefHandler.UnSubscribe(handler);
        }

        /// <summary>
        ///     发布结构体事件（ref传参）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RefPublish<T>(ref T e) where T : struct
        {
            RefHandler.Publish(ref e);
        }
        
        /// <summary>
        ///     添加结构体事件过滤器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRefFilter<T>(Func<T, Delegate, bool> filter) where T : struct
        {
            RefHandler.AddFilter(filter);
        }

        #endregion
    }
}