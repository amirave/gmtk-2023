using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fish;
using DefaultNamespace;

public class Sardine : FishAI
{
    [SerializeField] private float _speed;

    private GameManager _gm;

    protected override void Start()
    {
        base.Start();

        _gm = GameManager.Instance;
    }

    protected override void FishUpdate()
    {
        base.FishUpdate();

        transform.position = transform.position + transform.rotation * Vector3.right * _speed * _gm.difficulty.fishSpeed.GetCurrent(_gm.timeSinceStart) * Time.deltaTime;
    }
}
