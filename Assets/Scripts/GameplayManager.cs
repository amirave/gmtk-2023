using System;
using UI.Screens;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager Instance { get; private set; }

        [SerializeField] private float _topMargin;
        [SerializeField] private MenuScreen _hudScreen;
        [SerializeField] private float _scorePerSecond = 100;
        
        private Bounds _arenaBounds;
        private Camera _mainCam;
        private PhaseSpawner _phaseSpawner;
        private float _score;
        
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
        }

        void Start()
        {
            _phaseSpawner.Begin();
        }

        private void Update()
        {
            AddScore(Time.deltaTime * _scorePerSecond);
        }

        private void OnDrawGizmos()
        {
            float aspect = (float)Screen.width / Screen.height;
            float worldHeight = Camera.main.orthographicSize * 2;
            float worldWidth = worldHeight * aspect;

            _arenaBounds = new Bounds(new Vector2(0, -0.5f * _topMargin),
                new Vector2(worldWidth, worldHeight - _topMargin));
                
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_arenaBounds.center, _arenaBounds.extents * 2);
        }

        private void OnDestroy()
        {
            Instance = null;
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