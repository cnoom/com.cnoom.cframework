using System;

namespace CnoomFrameWork.Services.TimerService
{
    public class SecondsTimer : ISecondsTimer
    {
        
        private float _duration;
        private float _elapsed;
        private bool _isLoop;
        private bool _isCancelled;
        private bool _isPaused;
        private Action _callback;
        
        public float Duration => _duration;
        public float Elapsed => _elapsed;
        public bool IsCompleted => _isCancelled || (!_isLoop && _elapsed >= _duration);
        
        internal void Init(float delay, Action cb, bool loop) {
            _duration = delay;
            _callback = cb;
            _isLoop = loop;
            _elapsed = 0f;
            _isPaused = false;
            _isCancelled = false;
        }
        
        public void Update(float delta) {
            if (IsCompleted || _isPaused) return;
            _elapsed += delta;
            if (_elapsed >= _duration) {
                _callback?.Invoke();
                if (_isLoop) _elapsed = 0f;
                else {
                    _isCancelled = true;
                    // TimerService.Instance?.Recycle(this);
                }
            }
        }
        
        public void Cancel() {
            _isCancelled = true;
            // TimerService.Instance?.Recycle(this);
        }

        public void Pause() => _isPaused = true;
        public void Resume() => _isPaused = false;
    }
}