using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace Fish
{
    public class FishAI : MonoBehaviour
    {
        public enum FishState
        {
            Moving,
            Captured,
            Returning
        }

        [Header("Animation")] 
        [SerializeField] private float _captureDuration = 2f;
        [SerializeField] private float _captureShrinkSpins = 2f;
        
        protected SpriteRenderer sr;

        protected Vector3 initialPos;
        protected Quaternion initialRot;

        protected float _floor;
        protected float _rightSide;
        protected float _leftSide;

        protected FishState _state;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();

            initialPos = transform.position;
            initialRot = transform.rotation;
        }

        protected virtual void Start()
        {
            _floor = GameManager.Instance.GetArenaBounds().min.y;
            _leftSide = GameManager.Instance.GetArenaBounds().min.x;
            _rightSide = GameManager.Instance.GetArenaBounds().max.x;
        }

        private void Update()
        {
            if (_state == FishState.Captured)
                return;
            
            FishUpdate();
        }
        
        protected virtual void FishUpdate()
        {
            if (Mathf.Abs(transform.rotation.eulerAngles.z) >= 90 && Mathf.Abs(transform.rotation.eulerAngles.z) <= 270)
                sr.flipY = true;
            else
                sr.flipY = false;

            if (transform.position.y - sr.bounds.extents.y < _floor)
            {
                OnHitFloor();
            }
            if (transform.position.y + sr.bounds.extents.y > GameManager.Instance.GetArenaBounds().max.y)
            {
                OnLeaveWater();
            }
            if (transform.position.x - sr.bounds.extents.x < _leftSide)
            {
                OnHitLeftSide();
            }
            if (transform.position.x + sr.bounds.extents.x > _rightSide)
            {
                OnHitRightSide();
            }
        }

        protected virtual void OnHitFloor() { }

        protected virtual void OnHitLeftSide() { }

        protected virtual void OnHitRightSide() { }

        protected virtual async UniTask OnLeaveWater()
        {
            if (_state is FishState.Returning or FishState.Captured)
                return;

            _state = FishState.Returning;
            var initialRot = transform.rotation.eulerAngles.z;
            var targetRot = initialRot is < 90 and > -90 ? -1 * initialRot : (180 - initialRot) + 180;
            var finalRot = Quaternion.AngleAxis(targetRot, Vector3.forward);
            
            while (Mathf.Approximately(transform.rotation.eulerAngles.z, targetRot) == false)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRot, 30f * Time.deltaTime);
                await UniTask.Yield();
            }

            await UniTask.Delay(2000);

            _state = FishState.Moving;
        }
        
        public void Capture()
        {
            _state = FishState.Captured;
        }
        
        public async UniTask Consume()
        {
            var startTime = Time.time;
            var endTime = startTime + _captureDuration;
            var startRot = transform.rotation.eulerAngles.z;
            var startSize = transform.localScale;
            
            while (Time.time < endTime)
            {
                var t = Mathf.InverseLerp(startTime, endTime, Time.time);
                transform.localScale = Vector3.Lerp(startSize, Vector3.zero, t);
                transform.rotation = Quaternion.AngleAxis(Mathf.Lerp(startRot, startRot + 360 * _captureShrinkSpins, t), Vector3.forward);

                await UniTask.Yield();
            }
            
            Destroy(gameObject);
        }
    }
}