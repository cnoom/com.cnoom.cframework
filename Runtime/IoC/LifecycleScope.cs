namespace CnoomFrameWork.IoC
{
    /// <summary>
    ///     生命周期范围枚举
    /// </summary>
    public enum LifecycleScope
    {
        /// <summary>
        ///     单例模式，容器内保持唯一实例
        /// </summary>
        Singleton,
        /// <summary>
        ///     瞬时模式，每次解析创建新实例
        /// </summary>
        Transient
    }
}