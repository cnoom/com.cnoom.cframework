using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using CnoomFrameWork.Base.Log;
using UnityEngine;

namespace CnoomFrameWork.Base.Events
{
    /// <summary>
    ///     回调事件处理器，支持异步回调和结果返回
    /// </summary>
    public class CallbackEventHandler : TEventHandler
    {
        // 回调事件处理器委托定义
        public delegate void CallbackEvent<T, TResult>(T e, Action<TResult> callback);

        // 错误处理委托定义
        public delegate void ErrorHandler<TResult>(Exception exception, Action<TResult> callback);

        // 错误处理器字典
        private readonly Dictionary<Type, Delegate> _errorHandlers = new();

        /// <summary>
        ///     注册回调事件处理器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe<T, TResult>(CallbackEvent<T, TResult> handler, int priority = 0, bool once = false)
        {
            var type = typeof(CallbackEvent<T, TResult>);
            AddHandler(type, handler, priority, once);
        }

        /// <summary>
        ///     取消注册回调事件处理器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe<T, TResult>(CallbackEvent<T, TResult> handler)
        {
            var type = typeof(CallbackEvent<T, TResult>);
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
        {
            var type = typeof(CallbackEvent<T, TResult>);
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

            if (snapshot.Count > 1)
            {
                LogManager.Warn(nameof(CallbackEventHandler), "存在多个回调事件处理器，仅调用第一个匹配的处理器");
            }

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
                foreach (CallbackEventSubscriberAttribute attr in m
                             .GetCustomAttributes<CallbackEventSubscriberAttribute>())
                {
                    var parameters = m.GetParameters();
                    if (parameters.Length != 2 ||
                        !typeof(Delegate).IsAssignableFrom(parameters[1].ParameterType) &&
                        !parameters[1].ParameterType.IsGenericType)
                    {
                        LogManager.Error(nameof(CallbackEventHandler),
                            $"回调事件订阅方法 {m.Name} 参数格式错误，应为 (T e, Action<TResult> callback)");
                        continue;
                    }

                    // 处理泛型方法
                    if (m.IsGenericMethod)
                    {
                        RegisterGenericMethod(subscriber, m, attr);
                        continue;
                    }

                    // 处理非泛型方法
                    RegisterNonGenericMethod(subscriber, m, attr);
                }
            }
        }

        /// <summary>
        ///     注册泛型方法作为事件处理器
        /// </summary>
        private void RegisterGenericMethod(object subscriber, MethodInfo method, CallbackEventSubscriberAttribute attr)
        {
            // 获取方法的泛型参数
            var genericArgs = method.GetGenericArguments();
            if (genericArgs.Length == 0)
            {
                LogManager.Error(nameof(CallbackEventHandler), $"泛型方法 {method.Name} 没有泛型参数");
                return;
            }

            // 创建泛型方法适配器
            var adapterType = typeof(GenericMethodAdapter<>).MakeGenericType(genericArgs[0]);
            var adapter = Activator.CreateInstance(adapterType);

            // 使用反射调用适配器的Register方法
            var registerMethod = adapterType.GetMethod("Register");
            if (registerMethod == null)
            {
                LogManager.Error(nameof(CallbackEventHandler), $"无法找到适配器的Register方法");
                return;
            }

            try
            {
                registerMethod.Invoke(adapter, new object[] { this, subscriber, method, attr.Priority, attr.Once });
            }
            catch (Exception ex)
            {
                LogManager.Error(nameof(CallbackEventHandler), $"注册泛型方法 {method.Name} 失败: {ex.Message}");
            }
        }

        /// <summary>
        ///     注册非泛型方法作为事件处理器
        /// </summary>
        private void RegisterNonGenericMethod(object subscriber, MethodInfo method, CallbackEventSubscriberAttribute attr)
        {
            var parameters = method.GetParameters();
            var eventType = parameters[0].ParameterType;
            Type resultType;

            // 处理 Action<T> 类型的回调参数
            if (parameters[1].ParameterType.IsGenericType &&
                parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(Action<>))
            {
                resultType = parameters[1].ParameterType.GetGenericArguments()[0];
            }
            else
            {
                LogManager.Error(nameof(CallbackEventHandler),
                    $"回调事件订阅方法 {method.Name} 的回调参数类型不支持，必须是 Action<T> 类型");
                return;
            }

            try
            {
                var delegateType = typeof(CallbackEvent<,>).MakeGenericType(eventType, resultType);
                var del = Delegate.CreateDelegate(delegateType, subscriber, method, false);

                if (del == null)
                {
                    LogManager.Error(nameof(CallbackEventHandler), $"无法为方法 {method.Name} 创建委托，方法签名与委托类型不匹配");
                    return;
                }

                AddHandler(delegateType, del, attr.Priority, attr.Once);
            }
            catch (ArgumentException ex)
            {
                LogManager.Error(nameof(CallbackEventHandler), $"为方法 {method.Name} 创建委托失败: {ex.Message}");
            }
        }

        /// <summary>
        ///     泛型方法适配器，用于处理泛型方法的注册
        /// </summary>
        private class GenericMethodAdapter<T>
        {
            public void Register(CallbackEventHandler handler, object subscriber, MethodInfo method, int priority, bool once)
            {
                // 获取方法参数
                var parameters = method.GetParameters();
                var eventType = parameters[0].ParameterType;
                
                // 获取回调参数类型
                if (!parameters[1].ParameterType.IsGenericType || 
                    parameters[1].ParameterType.GetGenericTypeDefinition() != typeof(Action<>))
                {
                    LogManager.Error(nameof(CallbackEventHandler), 
                        $"泛型方法 {method.Name} 的回调参数类型不支持，必须是 Action<T> 类型");
                    return;
                }
                
                var resultType = parameters[1].ParameterType.GetGenericArguments()[0];
                
                // 创建泛型方法的具体实例
                var genericMethod = method.MakeGenericMethod(typeof(T));
                
                try
                {
                    // 创建委托
                    var delegateType = typeof(CallbackEvent<,>).MakeGenericType(eventType, resultType);
                    var del = Delegate.CreateDelegate(delegateType, subscriber, genericMethod, false);
                    
                    if (del == null)
                    {
                        LogManager.Error(nameof(CallbackEventHandler), 
                            $"无法为泛型方法 {method.Name}<{typeof(T).Name}> 创建委托");
                        return;
                    }
                    
                    // 添加处理器
                    handler.AddHandler(delegateType, del, priority, once);
                }
                catch (Exception ex)
                {
                    LogManager.Error(nameof(CallbackEventHandler), 
                        $"为泛型方法 {method.Name}<{typeof(T).Name}> 创建委托失败: {ex.Message}");
                }
            }
        }
    }
}