using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager Instance { get; private set; }

        [SerializeField] private float _topMargin;
        
        private Bounds _arenaBounds;
        private Camera _mainCam;
        private PhaseSpawner _phaseSpawner;
        
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
        }

        void Start()
        {
            _phaseSpawner.Begin();
        }

        private void Update()
        {
            
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

        public Bounds GetArenaBounds()
        {
            return _arenaBounds;
        }
    }
}