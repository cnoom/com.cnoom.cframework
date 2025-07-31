using System;

namespace CnoomFrameWork.Base.Timer
{
    public class SecondsTimer : ISecondsTimer
    {
        private Action _callback;
        private bool _isCancelled;
        private bool _isLoop;
        private bool _isPaused;

        private TimerService _timerService;

        public float Duration { get; private set; }

        public float Elapsed { get; private set; }

        public bool IsCompleted => _isCancelled || (!_isLoop && Elapsed >= Duration);
        public Action OnCancel { get; set; }

        public void Cancel()
        {
            OnCancel?.Invoke();
            OnCancel = null;
            _isCancelled = true;
            _timerService?.Recycle(this);
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }

        internal void Init(float delay, Action cb, bool loop, TimerService timerService)
        {
            _timerService = timerService;
            Duration = delay;
            _callback = cb;
            _isLoop = loop;
            Elapsed = 0f;
            _isPaused = false;
            _isCancelled = false;
            OnCancel = null;
        }

        public void Update(float delta)
        {
            if (IsCompleted || _isPaused) return;
            Elapsed += delta;
            if (Elapsed >= Duration)
            {
                _callback?.Invoke();
                if (_isLoop) Elapsed = 0f;
                else
                    Cancel();
            }
        }
    }
}