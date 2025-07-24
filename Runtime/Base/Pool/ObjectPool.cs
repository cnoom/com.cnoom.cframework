using System;
using System.Collections.Generic;

namespace CnoomFrameWork.Core.Base.Pool
{
    /// <summary>
    ///     通用泛型对象池（线程安全）
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public sealed class ObjectPool<T> : IDisposable where T : class
    {
        #region 构造函数

        /// <summary>
        ///     创建对象池实例
        /// </summary>
        /// <param name="createFunc">对象创建函数（必需）</param>
        /// <param name="onGet">从池中获取对象时的回调</param>
        /// <param name="onRelease">将对象放回池中时的回调</param>
        /// <param name="onDestroy">当对象被销毁时的回调</param>
        /// <param name="maxSize">对象池最大容量</param>
        public ObjectPool(
            Func<T> createFunc,
            Action<T> onGet = null,
            Action<T> onRelease = null,
            Action<T> onDestroy = null,
            int maxSize = 11)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _onGet = onGet;
            _onRelease = onRelease;
            _onDestroy = onDestroy;
            _maxSize = maxSize > 0
                ? maxSize
                : throw new ArgumentException("Max size must be greater than 0", nameof(maxSize));
            _pool = new Queue<T>(maxSize);
        }

        #endregion

        #region IDisposable实现

        public void Dispose()
        {
            lock (_lock)
            {
                if (_disposed) return;

                foreach (var item in _pool) _onDestroy?.Invoke(item);
                _pool.Clear();
                _disposed = true;
            }
        }

        #endregion

        #region 私有方法

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

        #region 私有字段

        private readonly Queue<T> _pool;
        private readonly Func<T> _createFunc;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly Action<T> _onDestroy;
        private readonly int _maxSize;
        private readonly object _lock = new();
        private int _countAll;
        private bool _disposed;

        #endregion

        #region 公共属性

        /// <summary>
        ///     总对象数量（包括已借出和池中的）
        /// </summary>
        public int CountAll
        {
            get
            {
                lock (_lock)
                {
                    return _countAll;
                }
            }
        }

        /// <summary>
        ///     池中可用的对象数量
        /// </summary>
        public int CountInactive
        {
            get
            {
                lock (_lock)
                {
                    return _pool.Count;
                }
            }
        }

        /// <summary>
        ///     当前借出的对象数量
        /// </summary>
        public int CountActive
        {
            get
            {
                lock (_lock)
                {
                    return _countAll - _pool.Count;
                }
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        ///     从池中获取对象
        /// </summary>
        public T Get()
        {
            lock (_lock)
            {
                ThrowIfDisposed();

                if (_pool.Count > 0)
                {
                    var obj = _pool.Dequeue();
                    _onGet?.Invoke(obj);
                    return obj;
                }

                var newObj = _createFunc();
                _countAll++;
                _onGet?.Invoke(newObj);
                return newObj;
            }
        }

        /// <summary>
        ///     将对象归还到池中
        /// </summary>
        public void Release(T element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            lock (_lock)
            {
                ThrowIfDisposed();

                if (_pool.Count < _maxSize)
                {
                    _onRelease?.Invoke(element);
                    _pool.Enqueue(element);
                }
                else
                {
                    _onDestroy?.Invoke(element);
                }
            }
        }

        /// <summary>
        ///     预生成对象到池中
        /// </summary>
        /// <param name="count">预生成数量</param>
        public void Prewarm(int count)
        {
            lock (_lock)
            {
                ThrowIfDisposed();

                count = Math.Min(count, _maxSize - _pool.Count);
                while (count-- > 0)
                {
                    var obj = _createFunc();
                    _countAll++;
                    _pool.Enqueue(obj);
                }
            }
        }

        /// <summary>
        ///     清空对象池并销毁所有池中对象
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                ThrowIfDisposed();

                foreach (var item in _pool) _onDestroy?.Invoke(item);
                _pool.Clear();
            }
        }

        #endregion
    }
}