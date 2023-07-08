using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fish
{
    public class FishSchool : MonoBehaviour
    {
        [SerializeField] private GameObject _head;

        private void Start()
        {
            transform.position -= _head.transform.position;
        }
    }
}
