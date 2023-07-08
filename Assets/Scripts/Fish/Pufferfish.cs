 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fish;
using DefaultNamespace;

public class Pufferfish : FishAI
{
    [SerializeField] private float _lapTime;

    [SerializeField] private float _verticalAccel;

    private float _horizontalSpeed;
    private float _length;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _length = GameplayManager.Instance.GetArenaBounds().extents.x * 2;
        _horizontalSpeed = _length / _lapTime;
        if (transform.right.x < 0 )
            _horizontalSpeed *= -1;
    }

    // Update is called once per frame
    protected override void FishUpdate()
    {
        base.FishUpdate();

        rb.velocity = new Vector3(_horizontalSpeed, rb.velocity.y + _verticalAccel * Time.deltaTime);
    }

    protected override void OnHitFloor()
    {
        if (rb.velocity.y < 0)
            rb.velocity = new Vector3(rb.velocity.x, -rb.velocity.y);
    }

    protected override void OnHitRightSide()
    {
        if (_horizontalSpeed > 0)
            _horizontalSpeed *= -1;
    }

    protected override void OnHitLeftSide()
    {
        print("hit left wall");
        if (_horizontalSpeed < 0)
            _horizontalSpeed *= -1;
    }
}
