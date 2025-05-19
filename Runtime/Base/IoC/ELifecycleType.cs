namespace CnoomFrameWork.Base.IoC
{
    /// <summary>
    ///     生命周期范围枚举
    /// </summary>
    public enum ELifecycleType
    {
        /// <summary>
        ///     每次获取都会创建一个新的实例
        /// </summary>
        Transient,
        /// <summary>
        ///     单例模式，只会创建一个实例
        /// </summary>
        Singleton
    }
}