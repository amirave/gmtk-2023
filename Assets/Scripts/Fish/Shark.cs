using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fish;
using DefaultNamespace;
using static UnityEngine.GraphicsBuffer;

public class Shark : FishAI
{
    private HookMovement _hook;

    [SerializeField] private float _speed;

    [SerializeField] private float _maxRotateDelta;

    private void Start()
    {
        _hook = GameplayManager.Instance.player;
    }

    void Update()
    {
        float angle = Mathf.Atan2(-transform.position.y + _hook.transform.position.y, -transform.position.x + _hook.transform.position.x) * Mathf.Rad2Deg;
        if (Vector3.Angle(transform.right, _hook.transform.position - transform.position) < 65)
            transform.right = Vector3.RotateTowards(transform.right, _hook.transform.position - transform.position, _maxRotateDelta * Time.deltaTime, 1);
        rb.velocity = transform.rotation * Vector3.right * _speed;
    }
}
