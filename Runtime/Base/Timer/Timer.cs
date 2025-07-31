using System;
using CnoomFrameWork.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CnoomFrameWork.Base.Timer
{
    /// <summary>
    /// 静态Timer类，提供便捷的定时器操作
    /// </summary>
    public static class Timer
    {
        private static TimerService _service;
        
        private static TimerService Service
        {
            get
            {
                if (_service == null)
                    _service = TimerService.Instance;
                return _service;
            }
        }

        #region 秒级定时器便捷方法

        /// <summary>
        /// 延迟执行（秒）
        /// </summary>
        /// <param name="delay">延迟时间（秒）</param>
        /// <param name="callback">回调函数</param>
        /// <returns>定时器实例</returns>
        public static SecondsTimer Delay(float delay, Action callback)
        {
            return Service.AddSecondsTimer(delay, callback, false);
        }

        /// <summary>
        /// 重复执行（秒）
        /// </summary>
        /// <param name="interval">间隔时间（秒）</param>
        /// <param name="callback">回调函数</param>
        /// <returns>定时器实例</returns>
        public static SecondsTimer Repeat(float interval, Action callback)
        {
            return Service.AddSecondsTimer(interval, callback, true);
        }

        /// <summary>
        /// 创建秒级定时器
        /// </summary>
        /// <param name="duration">持续时间（秒）</param>
        /// <param name="callback">回调函数</param>
        /// <param name="loop">是否循环</param>
        /// <returns>定时器实例</returns>
        public static SecondsTimer Seconds(float duration, Action callback, bool loop = false)
        {
            return Service.AddSecondsTimer(duration, callback, loop);
        }

        #endregion

        #region 帧级定时器便捷方法

        /// <summary>
        /// 延迟执行（帧）
        /// </summary>
        /// <param name="frames">延迟帧数</param>
        /// <param name="callback">回调函数</param>
        /// <returns>定时器实例</returns>
        public static FrameTimer DelayFrames(int frames, Action callback)
        {
            return Service.AddFrameTimer(frames, callback, false);
        }

        /// <summary>
        /// 重复执行（帧）
        /// </summary>
        /// <param name="interval">间隔帧数</param>
        /// <param name="callback">回调函数</param>
        /// <returns>定时器实例</returns>
        public static FrameTimer RepeatFrames(int interval, Action callback)
        {
            return Service.AddFrameTimer(interval, callback, true);
        }

        /// <summary>
        /// 创建帧级定时器
        /// </summary>
        /// <param name="duration">持续帧数</param>
        /// <param name="callback">回调函数</param>
        /// <param name="loop">是否循环</param>
        /// <returns>定时器实例</returns>
        public static FrameTimer Frames(int duration, Action callback, bool loop = false)
        {
            return Service.AddFrameTimer(duration, callback, loop);
        }

        /// <summary>
        /// 下一帧执行
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <returns>定时器实例</returns>
        public static FrameTimer NextFrame(Action callback)
        {
            return Service.AddFrameTimer(1, callback, false);
        }

        #endregion

        #region 特殊定时器方法

        /// <summary>
        /// 等待条件满足后执行
        /// </summary>
        /// <param name="condition">条件函数</param>
        /// <param name="callback">回调函数</param>
        /// <param name="timeout">超时时间（秒），-1表示无超时</param>
        /// <returns>定时器实例</returns>
        public static SecondsTimer WaitUntil(Func<bool> condition, Action callback, float timeout = -1)
        {
            var startTime = Time.time;
            SecondsTimer timer = null;
            timer = Repeat(0.02f, () =>
            {
                if (condition())
                {
                    callback?.Invoke();
                    timer?.Cancel();
                    return;
                }
                
                if (timeout > 0 && Time.time - startTime > timeout)
                {
                    // 超时处理
                    timer?.Cancel();
                    return;
                }
            });
            return timer;
        }

        /// <summary>
        /// 等待指定时间后执行，支持暂停游戏
        /// </summary>
        /// <param name="delay">延迟时间（秒）</param>
        /// <param name="callback">回调函数</param>
        /// <param name="useRealTime">是否使用真实时间（不受Time.timeScale影响）</param>
        /// <returns>定时器实例</returns>
        public static SecondsTimer DelayRealTime(float delay, Action callback, bool useRealTime = true)
        {
            if (!useRealTime)
                return Delay(delay, callback);

            var startTime = Time.realtimeSinceStartup;
            SecondsTimer timer = null;
            timer = Repeat(0.02f, () =>
            {
                if (Time.realtimeSinceStartup - startTime >= delay)
                {
                    callback?.Invoke();
                    timer?.Cancel();
                }
            });
            return timer;
        }

        #endregion

        #region 取消定时器方法

        /// <summary>
        /// 取消定时器
        /// </summary>
        /// <param name="timer">要取消的定时器</param>
        public static void Cancel(ITimer timer)
        {
            Service.RemoveTimer(timer);
        }

        /// <summary>
        /// 取消所有定时器
        /// </summary>
        public static void CancelAll()
        {
            Object.Destroy(Service);
        }

        #endregion
    }
}