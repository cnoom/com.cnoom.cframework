using System;
using System.Collections.Generic;
using CnoomFrameWork.Core;
using CnoomFrameWork.Core.UnityExtensions;
using CnoomFrameWork.Singleton;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CnoomFrameWork.Base.Timer
{
    public class TimerService : PersistentMonoSingleton<TimerService>
    {
        private readonly TimerCollections<FrameTimer> _frameTimers = new();
        private readonly TimerCollections<SecondsTimer> _secondsTimers = new();

        private void OnDestroy()
        {
            _secondsTimers.Dispose();
            _frameTimers.Dispose();
        }

        private void Update()
        {
            foreach (var timer in _frameTimers.Timers) timer.Update(Time.frameCount);

            foreach (var timer in _secondsTimers.Timers) timer.Update(Time.deltaTime);

            _frameTimers.RemoveCompleted();
            _secondsTimers.RemoveCompleted();

            _frameTimers.UpdateToAdd();
            _secondsTimers.UpdateToAdd();
        }

        public SecondsTimer AddSecondsTimer(float delay, Action callback, bool loop = false)
        {
            var t = _secondsTimers.Pool.Count > 0 ? _secondsTimers.Pool.Pop() : new SecondsTimer();
            t.Init(delay, callback, loop, this);
            _secondsTimers.AddToWait(t);
            return t;
        }

        public FrameTimer AddFrameTimer(int delay, Action callback, bool loop = false)
        {
            var t = _frameTimers.Pool.Count > 0 ? _frameTimers.Pool.Pop() : new FrameTimer();
            t.Init(delay, callback, loop, this);
            _frameTimers.AddToWait(t);
            return t;
        }

        public void RemoveTimer(ITimer target)
        {
            target?.Cancel();
        }

        internal void Recycle(ITimer t)
        {
            if (t == null) return;
            if (t is FrameTimer frameTimer) _frameTimers.Pool.Push(frameTimer);
            if (t is SecondsTimer secondsTimer) _secondsTimers.Pool.Push(secondsTimer);
        }

        private class TimerCollections<TTimer> : IDisposable where TTimer : ITimer
        {
            private readonly Queue<TTimer> _waitQueue = new();
            public readonly Stack<TTimer> Pool = new();
            public readonly List<TTimer> Timers = new();

            public void Dispose()
            {
                _waitQueue.Clear();
                Pool.Clear();
                Timers.Clear();
            }

            /// <summary>
            ///     添加计时器，会在更新时添加到Timers中。
            /// </summary>
            /// <param name="timer"></param>
            public void AddToWait(TTimer timer)
            {
                _waitQueue.Enqueue(timer);
            }

            /// <summary>
            ///     移除所有已完成的计时器。
            /// </summary>
            public void RemoveCompleted()
            {
                Timers.RemoveAll(t => t.IsCompleted);
            }

            /// <summary>
            ///     更新计时器，将_waitQueue中的计时器添加到Timers中。
            /// </summary>
            public void UpdateToAdd()
            {
                while (_waitQueue.Count > 0) Timers.Add(_waitQueue.Dequeue());
            }
        }
    }
}