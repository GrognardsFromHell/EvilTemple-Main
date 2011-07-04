using System;
using System.Collections.Generic;
using System.Diagnostics;
using EvilTemple.Runtime;
using EvilTemple.Runtime.Messages;

namespace EvilTemple.Rules
{
    public static class VisualTimers
    {
        private const int TicksPerMillisecond = 10000;

        private static readonly List<VisualTimerHandle> Timers;
        
        static VisualTimers()
        {
            Timers = new List<VisualTimerHandle>();
            EventBus.Register<DrawFrameMessage>(_ => Update());
        }

        private static void Update()
        {
            var now = DateTime.Now.Ticks / TicksPerMillisecond;

            var resort = false;

            List<Action> callbacks = null;

            for (var i = 0; i < Timers.Count; ++i)
            {
                if (Timers[i].Trigger > now)
                    break;

                if (callbacks == null)
                    callbacks = new List<Action>();

                callbacks.Add(Timers[i].Callback);

                if (!CallAgain(now, Timers[i]))
                    Timers.RemoveAt(i--);
                else
                    resort = true;
            }

            if (resort)
                SortTimers();

            if (callbacks != null)
                callbacks.ForEach(x => x());
        }

        /// <summary>
        /// Checks if a timer should be called again and updates the time at which it will trigger again.
        /// </summary>
        /// <param name="now">Current time in milliseconds.</param>
        /// <param name="visualTimer">Timer to check.</param>
        /// <returns>True if the timer will be called again.</returns>
        private static bool CallAgain(long now, VisualTimerHandle visualTimer)
        {
            if (visualTimer.Perpetual || (--visualTimer.RemainingCount) > 0)
            {
                visualTimer.Trigger = now + visualTimer.Interval;
                return true;
            }
            
            return false;
        }

        public static IVisualTimerHandle AddTimer(int interval, Action callback, bool perpetual = false, int remainingCount = 1)
        {
            var trigger = DateTime.Now.Ticks / TicksPerMillisecond + interval;

            var timer = new VisualTimerHandle(callback, interval, trigger, perpetual, remainingCount);
            Timers.Add(timer);

            SortTimers();

            return timer;
        }

        private static void SortTimers()
        {
            Timers.Sort((a, b) => a.Trigger.CompareTo(b.Trigger));
        }

        private static void Cancel(VisualTimerHandle handler)
        {
            Timers.Remove(handler);
        }

        private class VisualTimerHandle : IVisualTimerHandle
        {
            public Action Callback { get; private set; }

            public int Interval { get; private set; }

            public bool Perpetual { get; private set; }

            public long Trigger { get; set; }

            public int RemainingCount { get; set; }

            public VisualTimerHandle(
                Action callback, 
                int interval, 
                long trigger, 
                bool perpetual, 
                int remainingCount)
            {
                Callback = callback;
                Interval = interval;
                Trigger = trigger;
                Perpetual = perpetual;
                RemainingCount = remainingCount;
            }

            public bool IsCancelled
            {
                get { return !Timers.Contains(this); }
            }

            public void Cancel()
            {
                VisualTimers.Cancel(this);
            }
        }

    }

    public interface IVisualTimerHandle
    {
        bool IsCancelled { get; }

        void Cancel();
    }
    
}
