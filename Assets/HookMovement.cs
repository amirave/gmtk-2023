using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookMovement : MonoBehaviour
{
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private AnimationCurve _accelerationFactorFromDot;
    [SerializeField] private float _maxAcceleration;
    
    private Vector2 _goalVel;
    private Rigidbody2D _rb;


    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        var move = Vector2.ClampMagnitude(input, 1);
        
        var goalVel = move * _maxSpeed;
        var acceleration = _acceleration * _accelerationFactorFromDot.Evaluate(Vector2.Dot(move, _goalVel.normalized));
        
        _goalVel = Vector2.MoveTowards(_goalVel, goalVel, acceleration * Time.deltaTime);
        
        // var neededAccel = (_goalVel - _rb.velocity);
        // neededAccel = Vector2.ClampMagnitude(neededAccel, _maxAcceleration);
        //
        // // Debug.Log((_goalVel - _rb.velocity) / Time.deltaTime);
        // Debug.Log((_goalVel - _rb.velocity) / Time.deltaTime);
        _rb.AddForce(_goalVel, ForceMode2D.Impulse);
    }
}
