using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Timers
{
    public class TimerService : MonoBehaviour
    {
        public static TimerService Instance { get; private set; }
        
        private List<Timer> _timers;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            
            _timers = new List<Timer>();
        }

        public void RegisterTimer(Timer timer)
        {
            _timers.Add(timer);
        }

        public void RegisterTimer(float interval, Action callback, bool repeat = true)
        {
            _timers.Add(new Timer(interval, callback, repeat));
        }

        public void RemoveTimer(Timer timer)
        {
            _timers.Remove(timer);
        }

        private void Update()
        {
            foreach (var timer in _timers)
            {
                var status = timer.Tick();
                
                if (status == Timer.TimerStatus.Done)
                    RemoveTimer(timer);
            }
        }
    }
}