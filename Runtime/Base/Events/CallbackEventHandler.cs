using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CnoomFrameWork.Base.Events
{
    /// <summary>
    ///     回调事件处理器，支持异步回调和结果返回
    /// </summary>
    public class CallbackEventHandler : TEventHandler
    {
        // 回调事件处理器委托定义
        public delegate void CallbackEvent<in T, out TResult>(T e, Action<TResult> callback) where T : struct;
        
        // 错误处理委托定义
        public delegate void ErrorHandler<TResult>(Exception exception, Action<TResult> callback);

        // 错误处理器字典
        private readonly Dictionary<Type, Delegate> _errorHandlers = new();

        /// <summary>
        ///     注册回调事件处理器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe<T, TResult>(CallbackEvent<T, TResult> handler, int priority = 0, bool once = false)
            where T : struct
        {
            var type = typeof(CallbackEventWrapper);
            AddHandler(type, handler, priority, once);
        }

        /// <summary>
        ///     取消注册回调事件处理器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe<T, TResult>(CallbackEvent<T, TResult> handler)
            where T : struct
        {
            var type = typeof(CallbackEventWrapper);
            lock (WriteLock)
            {
                if (Handlers.TryGetValue(type, out var list))
                    list.RemoveAll(h => h.Handler == (Delegate)handler);
            }
        }

        /// <summary>
        ///     发布回调事件并等待结果
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish<T, TResult>(T e, Action<TResult> callback)
            where T : struct
        {

            var type = typeof(CallbackEventWrapper);
            List<HandlerInfo> snapshot;

            // 减少锁的范围，只在读取时加锁
            lock (ReadLock)
            {
                if (!Handlers.TryGetValue(type, out var list) || list.Count == 0)
                {
                    // 没有处理器，直接返回默认值
                    callback(default);
                    return;
                }

                snapshot = HandlerListPool.Get();
                snapshot.AddRange(list);
            }

            List<HandlerInfo> toRemove = null;
            bool hasHandler = false;

            // 遍历处理器并调用
            for (int i = 0; i < snapshot.Count; i++)
            {
                var h = snapshot[i];

                // 检查目标是否有效
                if (!h.Target.TryGetTarget(out var t) || (t is UnityEngine.Object o && !o))
                {
                    if (toRemove == null) toRemove = HandlerListPool.Get();
                    toRemove.Add(h);
                    continue;
                }

                // 检查过滤器
                if (!ShouldInvokeHandler(e, h.Handler)) continue;

                hasHandler = true;

                try
                {
                    // 处理同步回调
                    if (h.Handler is CallbackEvent<T, TResult> syncHandler)
                    {
                        syncHandler(e, callback);
                    }
                }
                catch (Exception ex)
                {
                    HandleCallbackError(ex, callback);
                }

                // 处理一次性事件
                if (h.Once)
                {
                    if (toRemove == null) toRemove = HandlerListPool.Get();
                    toRemove.Add(h);
                }

                // 只调用第一个匹配的处理器
                break;
            }

            // 如果没有找到有效的处理器，调用默认回调
            if (!hasHandler)
            {
                callback(default);
            }

            // 移除需要移除的处理器
            if (toRemove != null)
                TryRemoveHandle(type, toRemove);

            HandlerListPool.Release(snapshot);

            // 定期清理无效处理器
            CleanupDeadHandlers();
        }


        /// <summary>
        ///     处理回调错误
        /// </summary>
        private void HandleCallbackError<TResult>(Exception ex, Action<TResult> callback)
        {
            lock (ReadLock)
            {
                if (_errorHandlers.TryGetValue(typeof(TResult), out var handler))
                {
                    try
                    {
                        ((ErrorHandler<TResult>)handler)(ex, callback);
                        return;
                    }
                    catch
                    {
                        // 错误处理器异常，使用默认处理
                        Debug.LogError($"回调事件错误处理器异常: {ex.Message}");
                    }
                }
            }

            // 默认错误处理：返回默认值
            callback(default);
            Debug.LogError($"回调事件处理异常: {ex.Message}");
        }

        /// <summary>
        ///     扫描对象中带有特性的方法并自动注册为事件处理器
        /// </summary>
        public override void Register(object subscriber)
        {
            var methods = GetMethodInfo(subscriber);

            foreach (var m in methods)
            {
                foreach (var attr in m.GetCustomAttributes<CallbackEventSubscriberAttribute>())
                {
                    var parameters = m.GetParameters();
                    if (parameters.Length != 2 ||
                        parameters[1].ParameterType.GetGenericTypeDefinition() != typeof(Action<>))
                    {
                        Debug.LogError($"回调事件订阅方法 {m.Name} 参数格式错误，应为 (T e, Action<TResult> callback)");
                        continue;
                    }

                    var eventType = parameters[0].ParameterType;
                    var resultType = parameters[1].ParameterType.GetGenericArguments()[0];

                    var wrapperType = typeof(CallbackEventWrapper).MakeGenericType(eventType, resultType);
                    var delegateType = typeof(CallbackEvent<,>).MakeGenericType(eventType, resultType);
                    var del = GetOrCreateDelegate(delegateType, subscriber, m);

                    AddHandler(wrapperType, del, attr.Priority, attr.Once);
                }
            }
        }

        // 事件包装类，用于区分不同的事件类型和结果类型
        private class CallbackEventWrapper
        {
        }
    }
}