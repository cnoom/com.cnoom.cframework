namespace CnoomFrameWork.Base.Timer
{
    public interface ISecondsTimer : ITimer
    {
        /// <summary>
        ///     总时间,单位为秒
        /// </summary>
        float Duration { get; }

        /// <summary>
        ///     运行时间,单位为秒
        /// </summary>
        float Elapsed { get; }
    }
}