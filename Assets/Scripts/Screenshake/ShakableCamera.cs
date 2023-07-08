using System;
using Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Screenshake
{
    [RequireComponent(typeof(Camera))]
    public class ShakableCamera : MonoBehaviour
    {
        [SerializeField] private float _recoveryRate;
        [SerializeField] private float _magnitude = 0.1f;
        [SerializeField] private float _maxRotation = 10f;

        private float _trauma;
        private Camera _cam;

        private void Start()
        {
            _cam = GetComponent<Camera>();
            ScreenShakeService.Instance.RegisterCamera(this);
        }

        private void Update()
        {
            _trauma -= _recoveryRate * Time.deltaTime;
            _trauma = Mathf.Clamp01(_trauma);
        
            _cam.transform.localPosition = Utils.RandomDirection().ToVector3() * (_trauma * _trauma * _magnitude);

            var angle = _trauma * _trauma * _maxRotation;
            _cam.transform.localRotation = Quaternion.AngleAxis(Random.Range(-1 * angle, angle), Vector3.forward);
        }

        public void AddScreenShake(float intensity)
        {
            _trauma += intensity;
        }

        private void OnDestroy()
        {
            ScreenShakeService.Instance.RemoveCamera(this);
        }
    }
}