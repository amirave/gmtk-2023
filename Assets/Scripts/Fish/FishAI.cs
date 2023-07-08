using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
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
            Captured
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
            _floor = GameplayManager.Instance.GetArenaBounds().min.y;
            _leftSide = GameplayManager.Instance.GetArenaBounds().min.x;
            _rightSide = GameplayManager.Instance.GetArenaBounds().max.x;
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

        public void Capture()
        {
            _state = FishState.Captured;
        }
        
        public async UniTask Consume()
        {
            var startTime = Time.time;
            var endTime = startTime + _captureDuration;
            var startRot = transform.rotation.eulerAngles.z;
            
            while (Time.time < endTime)
            {
                var t = Mathf.InverseLerp(startTime, endTime, Time.time);
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
                transform.rotation *= Quaternion.AngleAxis(Mathf.Lerp(startRot, startRot + _captureShrinkSpins, t), Vector3.forward);

                await UniTask.Yield();
            }
            
            Destroy(gameObject);
        }
    }
}