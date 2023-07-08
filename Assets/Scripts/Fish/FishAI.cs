using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace Fish
{
    public class FishAI : MonoBehaviour
    {
        protected Rigidbody2D rb;
        protected SpriteRenderer sr;

        protected Vector3 initialPos;
        protected Quaternion initialRot;

        protected float _floor;
        protected float _rightSide;
        protected float _leftSide;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
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

        protected virtual void Update()
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

        public void Consume() { }
    }
}