using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fish;
using DefaultNamespace;

public class Shark : FishAI
{
    private HookMovement _hook;

    [SerializeField] private float _speed;

    [SerializeField] private float _maxRotateDelta;

    protected override void Start()
    {
        base.Start();

        _hook = GameplayManager.Instance.player;
    }

    protected override void FishUpdate()
    {
        base.FishUpdate();
        if (Vector3.Angle(transform.right, _hook.transform.position - transform.position) < 65)
            transform.right = Vector3.RotateTowards(transform.right, _hook.transform.position - transform.position, _maxRotateDelta * Time.deltaTime, 1);
        rb.velocity = transform.rotation * Vector3.right * _speed;
    }
}
