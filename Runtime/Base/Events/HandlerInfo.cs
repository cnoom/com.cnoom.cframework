using System;
using System.Runtime.CompilerServices;

namespace CnoomFrameWork.Base.Events
{
    /// <summary>
    ///     事件处理器信息，存储处理器的元数据
    /// </summary>
    public sealed class HandlerInfo : IEquatable<HandlerInfo>
    {
        /// <summary>
        ///     处理器委托
        /// </summary>
        public Delegate Handler;
        
        /// <summary>
        ///     是否为一次性处理器
        /// </summary>
        public bool Once;
        
        /// <summary>
        ///     处理器优先级
        /// </summary>
        public int Priority;
        
        /// <summary>
        ///     处理器目标对象的弱引用
        /// </summary>
        public WeakReference<object> Target;
        
        /// <summary>
        ///     处理器的唯一标识符，用于快速比较
        /// </summary>
        private int _hashCode;
        
        /// <summary>
        ///     计算并缓存处理器的哈希码
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ComputeHashCode()
        {
            _hashCode = Handler.GetHashCode();
        }
        
        /// <summary>
        ///     比较两个处理器信息是否相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(HandlerInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return ReferenceEquals(Handler, other.Handler);
        }
        
        /// <summary>
        ///     重写 Equals 方法
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is HandlerInfo other && Equals(other);
        }
        
        /// <summary>
        ///     重写 GetHashCode 方法
        /// </summary>
        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
