using System;
using System.Collections.Generic;

namespace CnoomFrameWork.Base.Container
{
    /// <summary>
    ///     容器基类，提供依赖注入和对象管理功能
    /// </summary>
    public abstract class BaseContainer : IDisposable
    {
        // 存储工厂方法的字典，用于创建特定类型的对象
        protected readonly Dictionary<Type, Func<BaseContainer, object>> Factories = new();

        // 用于线程安全的锁对象
        protected readonly object LockObject = new();

        // 存储作用域内对象的列表，用于统一释放资源
        protected readonly List<IDisposable> ScopedInstances = new();

        // 存储单例对象的字典，键为类型，值为对象实例
        protected readonly Dictionary<Type, object> Singletons = new();

        #region 注入方法

        private void Inject(object instance)
        {
            Injector.Inject(instance);
        }

        #endregion

        #region 注册方法

        /// <summary>
        ///     注册单例对象（生命周期与容器相同）
        /// </summary>
        public void BindSingleton<TInterface, TInstance>(TInstance instance)
        {
            lock (LockObject)
            {
                Singletons[typeof(TInterface)] = instance;

                // 如果对象实现了IDisposable接口，添加到作用域管理列表
                if (instance is IDisposable disposable) ScopedInstances.Add(disposable);
            }
        }

        /// <summary>
        ///     注册瞬态对象（每次请求都会创建新实例）
        /// </summary>
        public void BindTransient<TInterface, TInstance>(Func<BaseContainer, TInstance> factory)
        {
            lock (LockObject)
            {
                Factories[typeof(TInterface)] = container =>
                {
                    var instance = factory(container);

                    // 瞬态对象如果实现了IDisposable，也需要添加到作用域管理列表
                    if (instance is IDisposable disposable) ScopedInstances.Add(disposable);

                    return instance;
                };
            }
        }

        #endregion

        #region 取消注册的方法

        /// <summary>
        ///     解绑指定类型的单例对象，从容器中移除其注册信息
        /// </summary>
        /// <typeparam name="T">要解绑的单例对象的类型</typeparam>
        /// <returns>如果成功解绑返回 true，否则返回 false</returns>
        public bool UnBindSingleton<T>()
        {
            if (Singletons.TryGetValue(typeof(T), out var instance))
            {
                Singletons.Remove(typeof(T));
                return true;
            }

            return false;
        }

        /// <summary>
        ///     解绑指定类型的瞬态对象，从容器中移除其工厂方法注册信息
        /// </summary>
        /// <typeparam name="T">要解绑的瞬态对象的类型</typeparam>
        /// <returns>如果成功解绑返回 true，否则返回 false</returns>
        public bool UnBindTransient<T>()
        {
            if (Factories.TryGetValue(typeof(T), out var factory))
            {
                Factories.Remove(typeof(T));
                return true;
            }

            return false;
        }

        #endregion

        #region 解析方法

        /// <summary>
        ///     解析指定类型的对象实例
        /// </summary>
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        /// <summary>
        ///     解析指定类型的对象实例（非泛型版本）
        /// </summary>
        public virtual object Resolve(Type type)
        {
            // 首先检查当前容器的单例字典（读取操作不需要完全锁定）
            if (Singletons.TryGetValue(type, out var instance)) 
                return instance;

            // 然后检查当前容器的工厂方法字典
            Func<BaseContainer, object> factory = null;
            if (Factories.TryGetValue(type, out factory))
            {
                // 只在创建新实例时加锁，减少锁的范围
                var newInstance = factory(this);
                Injector.Inject(newInstance);
                return newInstance;
            }

            // 未找到注册项，返回null
            return null;
        }

        /// <summary>
        ///     尝试解析指定类型的对象实例
        /// </summary>
        public bool TryResolve<T>(out T instance)
        {
            try
            {
                instance = Resolve<T>();
                return true;
            }
            catch
            {
                instance = default;
                return false;
            }
        }

        #endregion

        #region 资源释放

        /// <summary>
        ///     释放容器管理的所有资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                lock (LockObject)
                {
                    // 逆序释放资源，确保依赖关系正确
                    for (var i = ScopedInstances.Count - 1; i >= 0; i--) ScopedInstances[i].Dispose();

                    ScopedInstances.Clear();
                    Singletons.Clear();
                    Factories.Clear();
                }
        }

        #endregion
    }
}