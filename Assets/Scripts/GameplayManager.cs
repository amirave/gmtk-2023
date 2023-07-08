using System;
using UI.Screens;
using UnityEngine;
using Managers;

namespace DefaultNamespace
{
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager Instance { get; private set; }

        [SerializeField] private float _topMargin;
        [SerializeField] private float _scorePerSecond = 100;
        [SerializeField] public HookMovement player;
        [SerializeField] private Sprite[] _emotionSprites;
        
        [SerializeField] private HudMenuScreen _hudScreen;
        [SerializeField] private DeathMenuScreen _deathScreen;
        
        private Bounds _arenaBounds;
        private Camera _mainCam;
        private PhaseSpawner _phaseSpawner;
        private float _score;
        
        private bool _isPlaying;
        
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

        void Start()
        {
            _phaseSpawner.Begin();
            _hudScreen.SetEmotion(_emotionSprites[0]);

            player.OnCaptureFish.AddListener(OnPlayerDamage);
            player.OnDeath.AddListener(OnPlayerDeath);
            
            _isPlaying = true;
        }

        private void Update()
        {
            if (_isPlaying)
                AddScore(Time.deltaTime * _scorePerSecond);
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
        }

        private void OnPlayerDeath()
        {
            _isPlaying = false;
            _hudScreen.gameObject.SetActive(false);
            _deathScreen.gameObject.SetActive(true);
            
            _deathScreen.SubscribeToButton("retry", () => GameManager.Instance.LoadScene(SceneIndexes.GAME));
            _deathScreen.SubscribeToButton("main_menu", () => GameManager.Instance.LoadScene(SceneIndexes.MAIN_MENU));
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
    }
}