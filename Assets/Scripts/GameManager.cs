﻿using System;
using Audio;
using Cysharp.Threading.Tasks;
using UI.Screens;
using UnityEngine;
using Managers;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private float _topMargin;
        [SerializeField] private float _scorePerSecond = 100;
        [SerializeField] public HookMovement player;
        [SerializeField] private Sprite[] _emotionSprites;
        
        [SerializeField] private Animator _tutorial;
        [SerializeField] private HudMenuScreen _hudScreen;
        [SerializeField] private DeathMenuScreen _deathScreen;

        [SerializeField] public DifficultySettings difficulty;
        
        private Bounds _arenaBounds;
        private Camera _mainCam;
        private PhaseSpawner _phaseSpawner;
        private float _score;
        
        private bool _isPlaying;

        public float timeSinceStart = 0;
        
        void Awake()
        {
            if (Instance == null)
                Instance = this;

            _mainCam = Camera.main;
            
            var aspect = (float)Screen.width / Screen.height;
            var worldHeight = _mainCam.orthographicSize * 2;
            var worldWidth = worldHeight * aspect;

            _arenaBounds = new Bounds(new Vector2(0, -0.5f * _topMargin),
                new Vector2(worldWidth, worldHeight - _topMargin));

            _phaseSpawner = GetComponentInChildren<PhaseSpawner>();

            Time.timeScale = 1;
            
            _hudScreen.gameObject.SetActive(true);
            _deathScreen.gameObject.SetActive(false);
        }

        async void Start()
        {
            AudioManager.Instance.PlayMusicTrack("game_theme_chill");
            _hudScreen.SetTextReplacement("score", "000000");
            
            await UniTask.Delay(1000);
            
            _tutorial.gameObject.SetActive(true);
            _tutorial.Play("Tutorial");
            
            await UniTask.Delay((int)(1000 * _tutorial.GetCurrentAnimatorStateInfo(0).length));
            
            _tutorial.gameObject.SetActive(false);
            
            _phaseSpawner.Begin();
            _hudScreen.SetEmotion(_emotionSprites[0]);

            player.OnCaptureFish.AddListener(OnPlayerDamage);
            player.OnDeath.AddListener(OnPlayerDeath);
            player._idleTime = 0;

            _isPlaying = true;
        }

        private void Update()
        {
            if (_isPlaying)
                AddScore(Time.deltaTime * _scorePerSecond);

            timeSinceStart += Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            var aspect = (float)Screen.width / Screen.height;
            var worldHeight = Camera.main.orthographicSize * 2;
            var worldWidth = worldHeight * aspect;

            _arenaBounds = new Bounds(new Vector2(0, -0.5f * _topMargin),
                new Vector2(worldWidth, worldHeight - _topMargin));
                
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_arenaBounds.center, _arenaBounds.extents * 2);
        }

        private void OnDestroy()
        {
            _phaseSpawner.Stop();
            Instance = null;
        }

        private void OnPlayerDamage(int health)
        {
            _hudScreen.SetEmotion(_emotionSprites[player.GetTotalHealth() - health]);
            if (health == 1)
                AudioManager.Instance.PlayMusicTrack("game_theme_intense");
        }

        private void OnPlayerDeath()
        {
            _isPlaying = false;
            _hudScreen.gameObject.SetActive(false);
            _deathScreen.gameObject.SetActive(true);
            
            _deathScreen.SubscribeToButton("retry", () => LifetimeManager.Instance.LoadScene(SceneIndexes.GAME));
            _deathScreen.SubscribeToButton("main_menu", () => LifetimeManager.Instance.LoadScene(SceneIndexes.MAIN_MENU));

            print(_score + ", " + ((int)_score).ToString());
            _deathScreen.SetTextReplacement("score", ((int)_score).ToString());
        }

        public Bounds GetArenaBounds()
        {
            return _arenaBounds;
        }
        
        public void AddScore(float amount)
        {
            _score += amount;
            _hudScreen.SetTextReplacement("score", ((int)_score).ToString().PadLeft(6, '0'));
        }

        public bool IsPlaying()
        {
            return _isPlaying;
        }
    }
}