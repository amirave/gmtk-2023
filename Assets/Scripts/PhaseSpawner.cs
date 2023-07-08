using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Fish;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhaseSpawner : MonoBehaviour
{
    [SerializeField] private DifficultySettings _difficultySettings;
    [SerializeField] private List<FishSpawnPattern> _spawnPatterns;
    
    private float _timeBetweenPhases = 8f;
    private int _obstaclePerPhase = 3;
    private float _timeBetweenObstacles = 1f;

    private bool _active = false;
    private float _lastPhaseEnd = 0;
    private bool _midPhase = false;
    private float _obstacleHeight;
    
    private GameplayManager gs => GameplayManager.Instance;

    public void Begin()
    {
        _lastPhaseEnd = Time.time;
        _active = true;
    }

    public void Stop()
    {
        _active = false;
    }

    private void Update()
    {
        if (_active == false)
            return;
        
        _obstaclePerPhase = 0; //(int) gs.difficultySettings.obstaclesPerPhase.GetCurrent(gs.GetTime());
        _timeBetweenObstacles = 0; //gs.difficultySettings.timeBetweenObstacles.GetCurrent(gs.GetTime());
        _timeBetweenPhases = 0; //gs.difficultySettings.timeBetweenPhases.GetCurrent(gs.GetTime());

        if (!_midPhase && Time.time - _lastPhaseEnd >= _timeBetweenPhases)
        {
            var pattern = _spawnPatterns.PickRandom();
            StartPhase(pattern);
        }
    }

    private async void StartPhase(FishSpawnPattern pattern)
    {
        _midPhase = true;
        var uniformSpawnRight = Random.value > 0.5f;
        
        foreach (var spawn in pattern.spawns)
        {
            var spawnRight = Random.value > 0.5f;

            var normalizedDir = spawn.spawnMode switch
            {
                FishSpawnMode.Left => -1,
                FishSpawnMode.Right => 1,
                FishSpawnMode.UniformRandom => uniformSpawnRight ? 1 : -1,
                _ => spawnRight ? 1 : -1
            };

            var spawnPos = new Vector3(normalizedDir * 1.2f, spawn.height);
            spawnPos.Scale(gs.GetArenaBounds().extents);
            spawnPos += gs.GetArenaBounds().center;

            var correctedAngle = normalizedDir * spawn.angle + (normalizedDir == -1 ? 180 : 0);
            var rot = Quaternion.AngleAxis(correctedAngle, Vector3.forward);
            
            Instantiate(spawn.school, spawnPos, rot, transform);

            await UniTask.Delay(TimeSpan.FromSeconds(spawn.delayUntilNext));
        }

        _midPhase = false;
        _lastPhaseEnd = Time.time;
    }
}
