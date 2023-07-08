 using System.Collections;
using System.Collections.Generic;
 using Audio;
 using UnityEngine;

using Fish;
using DefaultNamespace;

public class Pufferfish : FishAI
{
    [SerializeField] private float _lapTime;
    [SerializeField] private float _verticalAccel;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private int _bouncesTillLeave = 2;

    private float _horizontalSpeed;
    private float _length;

    private Vector3 _velocity;

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

        _velocity = new Vector2(_horizontalSpeed, _velocity.y + _verticalAccel * Time.deltaTime);
        transform.position += _velocity * Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(_rotationSpeed * Time.deltaTime, Vector3.forward);
    }

    protected override void OnHitFloor()
    {
        if (_velocity.y < 0)
            _velocity = new Vector2(_velocity.x, -_velocity.y);
        
        AudioManager.Instance.PlayEffect("pufferfish_bounce");
    }

    protected override void OnHitRightSide()
    {
        if (_bouncesTillLeave == 0)
            Destroy(gameObject, 2f);
        else if (_horizontalSpeed > 0)
        {
            _horizontalSpeed *= -1;
            _bouncesTillLeave -= 1;
            AudioManager.Instance.PlayEffect("pufferfish_bounce");
        }
    }

    protected override void OnHitLeftSide()
    {
        if (_bouncesTillLeave == 0)
            Destroy(gameObject, 2f);
        else if (_horizontalSpeed < 0)
        {
            _horizontalSpeed *= -1;
            _bouncesTillLeave -= 1;
            AudioManager.Instance.PlayEffect("pufferfish_bounce");
        }
    }
}
