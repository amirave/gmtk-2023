using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Screenshake
{
    public class ScreenShakeService : MonoBehaviour
    {
        public static ScreenShakeService Instance { get; private set; }
        
        private List<ShakableCamera> _cameras;
        
        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            
            _cameras = new List<ShakableCamera>();
        }

        public void RegisterCamera(ShakableCamera cam)
        {
            _cameras.Add(cam);
        }

        public void RemoveCamera(ShakableCamera cam)
        {
            _cameras.Remove(cam);
        }

        // TODO maybe add a position parameter so shake could only affect some cameras
        public void AddScreenShake(float intensity)
        {
            foreach (var cam in _cameras)
                cam.AddScreenShake(intensity);
        }
    }
}