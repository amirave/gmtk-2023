using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Fish;
using UnityEngine;
using UnityEngine.Events;

public class HookMovement : MonoBehaviour
{
    public enum PlayerState
    {
        Moving,
        Capturing,
        Dead
    }
    
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private AnimationCurve _accelerationFactorFromDot;
    [SerializeField] private int _totalHealth = 4;

    [Header("Capture Animation")] 
    [SerializeField] private float _captureMoveSpeed = 0.1f;
    [SerializeField] private float _captureHoldDuration = 0.5f;
    [SerializeField] private float _invincibilityTime = 1f;
    [SerializeField] private float _blinkInterval = 0.4f;
    
    private Vector2 _goalVel;
    private bool _invincible;
    private int _currentHealth;
    
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    [HideInInspector] public PlayerState playerState = PlayerState.Moving;

    [HideInInspector] public UnityEvent<int> OnCaptureFish;
    [HideInInspector] public UnityEvent OnDeath;

    private void Awake()
    {
        _currentHealth = _totalHealth;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (playerState == PlayerState.Moving)
            MovePlayer();
        
        if ((_invincible && playerState == PlayerState.Capturing) && 
                Time.time % _blinkInterval > 0.5f * _blinkInterval)
            _spriteRenderer.color = Color.clear;
        else
            _spriteRenderer.color = Color.white;

        if (Input.GetKeyDown(KeyCode.R))
            CaptureFish(null);
    }

    private void MovePlayer()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        var move = Vector2.ClampMagnitude(input, 1);
        
        var goalVel = move * _maxSpeed;
        var acceleration = _acceleration * _accelerationFactorFromDot.Evaluate(Vector2.Dot(move, _goalVel.normalized));
        
        _goalVel = Vector2.MoveTowards(_goalVel, goalVel, acceleration * Time.deltaTime);
        _rb.AddForce(_goalVel, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        var fish = col.gameObject.GetComponent<FishAI>();

        if (fish == null)
            return;

        if (_invincible)
            return;

        CaptureFish(fish);
    }

    private async UniTask CaptureFish(FishAI fish)
    {
        playerState = PlayerState.Capturing;
        _invincible = true;
        
        // TODO indicator and sfx
        AudioManager.Instance.PlayEffect("fish_hurt");

        await UniTask.Delay(100);

        var gm = GameplayManager.Instance;
        var origPos = transform.position;
        
        var prevTime = Time.time;
        while (transform.position.y < gm.GetArenaBounds().max.y)
        {
            var deltaTime = Time.time - prevTime;
            transform.position += Vector3.up * (_captureMoveSpeed * deltaTime);
            prevTime = Time.time;

            await UniTask.Yield();
        }

        await UniTask.Delay(1000 * (int)_captureHoldDuration);
        _currentHealth -= 1;
        OnCaptureFish.Invoke(_currentHealth);
        if (_currentHealth == 0)
        {
            OnDeath.Invoke();
            playerState = PlayerState.Dead;
            return;
        }

        prevTime = Time.time;
        while (transform.position.y > origPos.y)
        {
            var deltaTime = Time.time - prevTime;
            transform.position += Vector3.down * (_captureMoveSpeed * deltaTime);
            prevTime = Time.time;

            await UniTask.Yield();
        }

        playerState = PlayerState.Moving;
        _invincible = true;
        await UniTask.Delay(1000 * (int)_invincibilityTime);
        _invincible = false;
    }

    public int GetTotalHealth()
    {
        return _totalHealth;
    }
}
