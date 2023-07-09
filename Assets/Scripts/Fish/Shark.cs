using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        _hook = GameManager.Instance.player;
    }

    protected override void FishUpdate()
    {
        base.FishUpdate();

        var dir = Vector3.left;
        if (Vector3.Angle(transform.right, Vector3.right) <= 90)
            dir = Vector3.right;

        if (Vector3.Angle(dir, _hook.transform.position - transform.position) < 25)
            transform.right = Vector3.RotateTowards(transform.right, _hook.transform.position - transform.position, _maxRotateDelta * Time.deltaTime, 1);
        
        transform.position += transform.right * (_speed * Time.deltaTime);
    }
}
