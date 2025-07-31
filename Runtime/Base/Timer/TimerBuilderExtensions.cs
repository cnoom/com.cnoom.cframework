using System;
using UnityEngine;

namespace CnoomFrameWork.Base.Timer
{
    /// <summary>
    /// 定时器构建器扩展方法，支持链式调用
    /// </summary>
    public static class TimerBuilderExtensions
    {
        #region SecondsTimer 扩展方法

        /// <summary>
        /// 绑定到GameObject，当GameObject销毁时自动取消定时器
        /// </summary>
        /// <param name="timer">定时器</param>
        /// <param name="gameObject">要绑定的GameObject</param>
        /// <returns>定时器实例</returns>
        public static SecondsTimer BindTo(this SecondsTimer timer, GameObject gameObject)
        {
            timer.Bind(gameObject);
            return timer;
        }

        /// <summary>
        /// 设置取消回调
        /// </summary>
        /// <param name="timer">定时器</param>
        /// <param name="onCancel">取消时的回调</param>
        /// <returns>定时器实例</returns>
        public static SecondsTimer SetCancel(this SecondsTimer timer, Action onCancel)
        {
            timer.OnCancel = onCancel;
            return timer;
        }

        /// <summary>
        /// 立即暂停定时器
        /// </summary>
        /// <param name="timer">定时器</param>
        /// <returns>定时器实例</returns>
        public static SecondsTimer PauseImmediately(this SecondsTimer timer)
        {
            timer.Pause();
            return timer;
        }

        /// <summary>
        /// 延迟指定时间后开始执行
        /// </summary>
        /// <param name="timer">定时器</param>
        /// <param name="startDelay">开始延迟时间</param>
        /// <returns>定时器实例</returns>
        public static SecondsTimer StartAfter(this SecondsTimer timer, float startDelay)
        {
            timer.Pause();
            Timer.Delay(startDelay, timer.Resume);
            return timer;
        }

        #endregion

        #region FrameTimer 扩展方法

        /// <summary>
        /// 绑定到GameObject，当GameObject销毁时自动取消定时器
        /// </summary>
        /// <param name="timer">定时器</param>
        /// <param name="gameObject">要绑定的GameObject</param>
        /// <returns>定时器实例</returns>
        public static FrameTimer BindTo(this FrameTimer timer, GameObject gameObject)
        {
            timer.Bind(gameObject);
            return timer;
        }

        /// <summary>
        /// 设置取消回调
        /// </summary>
        /// <param name="timer">定时器</param>
        /// <param name="onCancel">取消时的回调</param>
        /// <returns>定时器实例</returns>
        public static FrameTimer SetCancel(this FrameTimer timer, Action onCancel)
        {
            timer.OnCancel = onCancel;
            return timer;
        }

        /// <summary>
        /// 立即暂停定时器
        /// </summary>
        /// <param name="timer">定时器</param>
        /// <returns>定时器实例</returns>
        public static FrameTimer PauseImmediately(this FrameTimer timer)
        {
            timer.Pause();
            return timer;
        }

        /// <summary>
        /// 延迟指定帧数后开始执行
        /// </summary>
        /// <param name="timer">定时器</param>
        /// <param name="startDelayFrames">开始延迟帧数</param>
        /// <returns>定时器实例</returns>
        public static FrameTimer StartAfter(this FrameTimer timer, int startDelayFrames)
        {
            timer.Pause();
            Timer.DelayFrames(startDelayFrames, timer.Resume);
            return timer;
        }

        #endregion

        #region 通用扩展方法

        /// <summary>
        /// 条件执行 - 只有当条件为真时才执行定时器
        /// </summary>
        /// <param name="timer">定时器</param>
        /// <param name="condition">条件函数</param>
        /// <returns>定时器实例</returns>
        public static T ExecuteIf<T>(this T timer, Func<bool> condition) where T : ITimer
        {
            if (!condition())
            {
                timer.Cancel();
            }
            return timer;
        }

        /// <summary>
        /// 自动取消 - 在指定时间后自动取消定时器
        /// </summary>
        /// <param name="timer">定时器</param>
        /// <param name="autoCancel">自动取消时间（秒）</param>
        /// <returns>定时器实例</returns>
        public static T AutoCancel<T>(this T timer, float autoCancel) where T : ITimer
        {
            Timer.Delay(autoCancel, timer.Cancel);
            return timer;
        }

        #endregion
    }
}