using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fish;

public class Sardine : FishAI
{
    [SerializeField] private float _speed;

    protected override void Start()
    {
        base.Start();


    }

    protected override void FishUpdate()
    {
        base.FishUpdate();

        transform.position = transform.position + transform.rotation * Vector3.right * _speed * Time.deltaTime;
    }
}
