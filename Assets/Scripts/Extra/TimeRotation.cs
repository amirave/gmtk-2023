using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TimeRotation : MonoBehaviour
{
    public float speed = 1f;
    public Vector3 axis = Vector3.one;

    void FixedUpdate()
    {
        transform.RotateAround(transform.position, axis, speed);
    }
}
