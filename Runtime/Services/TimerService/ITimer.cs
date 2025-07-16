using System;

namespace CnoomFrameWork.Services.TimerService
{
    public interface ITimer
    {
        bool IsCompleted { get; }
        Action OnCancel { get; set; }

        /// <summary>
        ///     取消定时器
        /// </summary>
        void Cancel();

        /// <summary>
        ///     暂停定时器
        /// </summary>
        void Pause();

        /// <summary>
        ///     恢复定时器
        /// </summary>
        void Resume();
    }

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