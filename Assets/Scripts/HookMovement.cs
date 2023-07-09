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
        DodgeRolling,
        Capturing,
        Dead
    }
    
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private AnimationCurve _accelerationFactorFromDot;
    [SerializeField] private int _totalHealth = 4;
    
    [Header("Dodge Roll")]
    [SerializeField] private float _dodgeRollTime;
    [SerializeField] private AnimationCurve _dodgeRollAccelerationCurve;
    [SerializeField] private float _dodgeRollAccelerationMultiplier;
    [SerializeField] private float _dodgeRollCooldown;
    [SerializeField] private float _gracePeriod;

    [SerializeField] private LineRenderer _ropeLine;

    [Header("Capture Animation")] 
    [SerializeField] private float _captureMoveSpeed = 0.1f;
    [SerializeField] private float _captureHoldDuration = 0.5f;
    [SerializeField] private float _invincibilityTime = 1f;
    [SerializeField] private float _blinkInterval = 0.4f;
    
    private Vector2 _goalVel;
    private bool _invincible;
    private int _currentHealth;
    private float _lastDodgeRollTime = 0;
    private bool _canDodgeRoll = true;
    public float _idleTime;
    
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
        {
            if (Input.GetKeyDown(KeyCode.Space) && _canDodgeRoll)
            {
                _lastDodgeRollTime = Time.time;
                DodgeRoll();
                _canDodgeRoll = false;
            }
            else
                MovePlayer();
            CheckBounds();
        }
        if (playerState == PlayerState.DodgeRolling)
            CheckBounds();

        if (Time.time - _lastDodgeRollTime >= _dodgeRollCooldown && !_canDodgeRoll)
        {
            Flash();
            _canDodgeRoll = true;
        }

        if ((_invincible && playerState == PlayerState.Moving) && 
                Time.time % _blinkInterval > 0.5f * _blinkInterval)
            _spriteRenderer.color = Color.clear;
        else
            _spriteRenderer.color = Color.white;

        _ropeLine.SetPositions(new[]{transform.position + Vector3.up * 0.5f, new Vector3(1.14f, 5.01f)});
    }

    private void CheckBounds()
    {
        var pos = transform.position;
        var gm = GameManager.Instance;
        var bounds = gm.GetArenaBounds();
        bounds.size *= 0.9f;
        
        if (pos.y > bounds.max.y)
        {
            pos.y = bounds.max.y;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Min(_rb.velocity.y, 0));
        }
        else if (pos.y < bounds.min.y)
        {
            pos.y = bounds.min.y;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, 0));
        }
        
        if (pos.x > bounds.max.x)
        {
            pos.x = bounds.max.x;
            _rb.velocity = new Vector2(Mathf.Min(_rb.velocity.x, 0), _rb.velocity.y);
        }
        else if (pos.x < bounds.min.x)
        {
            pos.x = bounds.min.x;
            _rb.velocity = new Vector2(Mathf.Max(_rb.velocity.x, 0), _rb.velocity.y);
        }
    }

    private void MovePlayer()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        var move = Vector2.ClampMagnitude(input, 1);

        if (move == Vector2.zero)
            _idleTime += Time.deltaTime;
        else
            _idleTime = 0;

        var goalVel = move * _maxSpeed;
        var acceleration = _acceleration * _accelerationFactorFromDot.Evaluate(Vector2.Dot(move, _goalVel.normalized));
        
        _goalVel = Vector2.MoveTowards(_goalVel, goalVel, acceleration * Time.deltaTime);
        _rb.AddForce(_goalVel, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var fish = col.gameObject.GetComponent<FishAI>();

        if (fish == null)
            return;

        if (_invincible || playerState != PlayerState.Moving)
            return;

        CaptureFish(fish);
    }

    private async UniTask CaptureFish(FishAI fish)
    {
        playerState = PlayerState.Capturing;
        _invincible = true;
        
        // TODO indicator and sfx
        AudioManager.Instance.PlayEffect("fish_hurt");

        // Small wait
        fish.Capture();
        await UniTask.Delay(100);

        var gm = GameManager.Instance;
        var origPos = transform.position;
        
        // Start lifting the hook
        var prevTime = Time.time;
        while (transform.position.y < gm.GetArenaBounds().max.y)
        {
            var deltaTime = Time.time - prevTime;
            transform.position += Vector3.up * (_captureMoveSpeed * deltaTime);

            fish.transform.position = Vector3.MoveTowards(fish.transform.position,
                GetComponent<Collider2D>().bounds.center, 1f * deltaTime);
            fish.transform.position += Vector3.up * (_captureMoveSpeed * deltaTime);

            prevTime = Time.time;

            await UniTask.Yield();
        }

        // Wait for consume animation then hurt player
        fish.Consume();
        await UniTask.Delay((int)(1000 * _captureHoldDuration));
        _currentHealth -= 1;
        OnCaptureFish.Invoke(_currentHealth);
        if (_currentHealth == 0)
        {
            OnDeath.Invoke();
            playerState = PlayerState.Dead;
            return;
        }

        // Cast back
        prevTime = Time.time;
        while (transform.position.y > origPos.y)
        {
            var deltaTime = Time.time - prevTime;
            transform.position += Vector3.down * (_captureMoveSpeed * deltaTime);
            prevTime = Time.time;

            await UniTask.Yield();
        }

        // Give and then remove invincibility
        playerState = PlayerState.Moving;
        _invincible = true;
        await UniTask.Delay((int)(1000 * _invincibilityTime));
        _invincible = false;
    }

    private async void DodgeRoll()
    {
        Quaternion startRotation = transform.rotation;
        playerState = PlayerState.DodgeRolling;
        Vector3 direction = _rb.velocity.normalized;
        float startTIme = Time.time;
        _invincible = true;

        while (Time.time - startTIme < _dodgeRollTime) 
        {
            _rb.AddForce(_rb.mass * _dodgeRollAccelerationCurve.Evaluate((Time.time - startTIme) / _dodgeRollTime) * _dodgeRollAccelerationMultiplier * direction);
            transform.rotation *= Quaternion.AngleAxis(1, Vector3.up);

            await UniTask.Yield();
        }

        transform.rotation = startRotation;
        playerState = PlayerState.Moving;
        
        await UniTask.Delay((int)(1000 * _gracePeriod));
        _invincible = false;
    }

    private async void Flash()
    {
        float flashAmount = 0;
        float elapsedTime = 0;

        while (elapsedTime < 0.05)
        {
            elapsedTime += Time.deltaTime;

            flashAmount = Mathf.Lerp(0, 1, elapsedTime / 0.05f);

            _spriteRenderer.material.SetFloat("_FlashAmount", flashAmount);

            await UniTask.Yield();
        }

        while (elapsedTime < 0.1)
        {
            elapsedTime += Time.deltaTime;

            flashAmount = Mathf.Lerp(1, 0, (elapsedTime - 0.05f) / 0.05f);

            _spriteRenderer.material.SetFloat("_FlashAmount", flashAmount);

            await UniTask.Yield();
        }
    }

    public int GetTotalHealth()
    {
        return _totalHealth;
    }
}
