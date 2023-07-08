using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fish;

public class Sardine : FishAI
{
    [SerializeField] private float speed;

    protected override void Start()
    {
        base.Start();

        rb.velocity = transform.rotation * Vector3.right * speed;
    }
}
