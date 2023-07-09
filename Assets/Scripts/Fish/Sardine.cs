using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fish;
using DefaultNamespace;

public class Sardine : FishAI
{
    [SerializeField] private float _speed;

    private DifficultySettings _difficulty;

    protected override void Start()
    {
        base.Start();

        _difficulty = GameManager.Instance.difficulty;
    }

    protected override void FishUpdate()
    {
        base.FishUpdate();

        transform.position = transform.position + transform.rotation * Vector3.right * _speed * _difficulty.fishSpeed.GetCurrent(Time.time) * Time.deltaTime;
    }
}
