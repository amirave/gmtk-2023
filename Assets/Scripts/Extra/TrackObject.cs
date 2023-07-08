using System;
using UnityEngine;

namespace Extra
{
    public class TrackObject : MonoBehaviour
    {
        public Transform tracked;
        
        public bool trackPosition;
        public bool trackRotation;

        private Vector3 _posOffset;
        private Quaternion _rotOffset;
        
        private void Start()
        {
            _posOffset = transform.position - tracked.position;
            _rotOffset = Utils.ShortestRotation(tracked.rotation, transform.rotation);
        }

        private void Update()
        {
            if (trackPosition)
                transform.position = tracked.position + _posOffset;
            if (trackRotation)
                 transform.rotation = tracked.rotation * _rotOffset;
        }
    }
}