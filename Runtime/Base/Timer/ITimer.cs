using System;

namespace CnoomFrameWork.Base.Timer
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
}