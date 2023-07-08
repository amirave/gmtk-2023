using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fish
{
    public class FishAI : MonoBehaviour
    {
        protected Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public virtual void OnHitFloor()
        {
            Destroy(gameObject);
        }
    }
}