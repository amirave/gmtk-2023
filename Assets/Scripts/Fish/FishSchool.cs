using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fish
{
    public class FishSchool : MonoBehaviour
    {
        [SerializeField] private GameObject _head;

        private FishAI[] fishes;

        private void Start()
        {
            transform.position = _head.transform.position;
        }

        private void Update()
        {
            if (transform.childCount == 1)
            {
                Destroy(gameObject);
            }
        }
    }
}
