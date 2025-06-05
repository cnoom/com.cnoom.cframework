using System;

namespace CnoomFrameWork.Services.TimerService
{
    public class FrameTimer : IFrameTimer
    {
        private int _duration;
        private int _elapsed;
        private bool _isLoop;
        private bool _isCancelled;
        private bool _isPaused;
        private Action _callback;
        private TimerService _timerService;


        public int Duration => _duration;
        public int Elapsed => _elapsed;
        public bool IsCompleted => _isCancelled || (!_isLoop && _elapsed >= _duration);
        public Action OnCancel { get; set; }

        internal void Init(int delay, Action cb, bool loop,TimerService timerService)
        {
            _timerService = timerService;
            _duration = delay;
            _callback = cb;
            _isLoop = loop;
            _elapsed = 0;
            _isPaused = false;
            _isCancelled = false;
            OnCancel = null;
        }

        public void Update(int frame)
        {
            if (IsCompleted || _isPaused) return;
            _elapsed += frame;
            if (_elapsed >= _duration)
            {
                _callback?.Invoke();
                if (_isLoop) _elapsed = 0;
                else
                {
                    Cancel();
                }
            }
        }

        public void Cancel()
        {
            OnCancel?.Invoke();
            OnCancel = null;
            _isCancelled = true;
            _timerService?.Recycle(this);
        }

        public void Pause() => _isPaused = true;
        public void Resume() => _isPaused = false;
    }
}