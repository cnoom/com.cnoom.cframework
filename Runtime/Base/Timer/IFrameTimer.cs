namespace CnoomFrameWork.Base.Timer
{
    public interface IFrameTimer : ITimer
    {
        /// <summary>
        ///     总时间,单位为帧
        /// </summary>
        int Duration { get; }

        /// <summary>
        ///     运行时间,单位为帧
        /// </summary>
        int Elapsed { get; }
    }
}