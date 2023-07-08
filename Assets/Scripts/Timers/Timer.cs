using System;
using Core;
using UnityEngine;

namespace Timers
{
    public class Timer
    {
        public enum TimerStatus
        {
            Ongoing,
            Done
        }
        
        private float _interval;
        private Action _callback;
        private bool _repeat;

        private float _lastCallback;
        
        public Timer(float interval, Action callback, bool repeat = true)
        {
            _interval = interval;
            _callback = callback;
            _repeat = repeat;
            
            _lastCallback = Time.time;
        }

        public TimerStatus Tick()
        {
            if (Time.time - _lastCallback > _interval)
            {
                _callback.Invoke();
                _lastCallback = Time.time;

                if (_repeat == false)
                    return TimerStatus.Done;
            }

            return TimerStatus.Ongoing;
        }
    }
}