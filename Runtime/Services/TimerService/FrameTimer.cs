using System;

namespace CnoomFrameWork.Services.TimerService
{
    public class FrameTimer : IFrameTimer
    {
        private Action _callback;
        private bool _isCancelled;
        private bool _isLoop;
        private bool _isPaused;
        private TimerService _timerService;


        public int Duration { get; private set; }

        public int Elapsed { get; private set; }

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

        internal void Init(int delay, Action cb, bool loop, TimerService timerService)
        {
            _timerService = timerService;
            Duration = delay;
            _callback = cb;
            _isLoop = loop;
            Elapsed = 0;
            _isPaused = false;
            _isCancelled = false;
            OnCancel = null;
        }

        public void Update(int frame)
        {
            if (IsCompleted || _isPaused) return;
            Elapsed += frame;
            if (Elapsed >= Duration)
            {
                _callback?.Invoke();
                if (_isLoop) Elapsed = 0;
                else
                    Cancel();
            }
        }
    }
}