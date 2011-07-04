using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilTemple.Rules
{
    public class CrossFader
    {
        public event Action<float> OnProgress;

        public event Action OnFinish;

        private long _fadeStart;

        private long _fadeEnd;

        private int _duration;

        private IVisualTimerHandle _timerHandle;

        public void CrossFade(int duration)
        {
            if (_timerHandle != null)
                _timerHandle.Cancel();

            _fadeStart = DateTime.Now.Ticks/10000;
            _duration = duration;

            _timerHandle = VisualTimers.AddTimer(50, Update, true);
        }

        private void Update()
        {
            if (_timerHandle == null)
                return;

            var now = DateTime.Now.Ticks/10000;

            var elapsed = (now - _fadeStart);
            var completion = elapsed/(float)_duration;

            if (completion >= 1.0f)
            {
                _timerHandle.Cancel();
                _timerHandle = null;
                OnFinish();
            }
            else
            {
                OnProgress(completion);
            }
        }
    }
}
