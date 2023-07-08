using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Fish;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhaseSpawner : MonoBehaviour
{
    private DifficultySettings _difficultySettings;
    [SerializeField] private List<FishSpawnPattern> _easySpawnPatterns;
    [SerializeField] private List<FishSpawnPattern> _medSpawnPatterns;
    [SerializeField] private List<FishSpawnPattern> _hardSpawnPatterns;

    private float _timeBetweenPhases = 8f;
    private int _obstaclePerPhase = 3;
    private float _timeBetweenObstacles = 1f;

    private bool _active = false;
    private float _lastPhaseEnd = 0;
    private bool _midPhase = false;
    private float _obstacleHeight;
    
    private GameplayManager gs => GameplayManager.Instance;

    private float _now;

    public void Begin()
    {
        _difficultySettings = gs.difficulty;
        _lastPhaseEnd = Time.time;
        _active = true;

        _now = 0;
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

        _timeBetweenPhases = 1/_difficultySettings.phaseRate.GetCurrent(_now);

        if (!_midPhase && Time.time - _lastPhaseEnd >= _timeBetweenPhases)
            StartPhase(GetRandomSpawnPattern());

        _now += Time.deltaTime;
    }

    private FishSpawnPattern GetRandomSpawnPattern()
    {
        int patternDifficulty = (int)(_difficultySettings.phaseDifficulty.GetCurrent(_now) * 3);
        int totalLength = _easySpawnPatterns.Count;
        if (patternDifficulty > 0)
            totalLength += _medSpawnPatterns.Count;
        if (patternDifficulty > 1)
            totalLength += _hardSpawnPatterns.Count;

        int i = Random.Range(0, totalLength);

        if (i < _easySpawnPatterns.Count)
            return _easySpawnPatterns[i];
        i -= _easySpawnPatterns.Count;
        if (i < _medSpawnPatterns.Count)
            return _medSpawnPatterns[i];
        i -= _medSpawnPatterns.Count;
        return _hardSpawnPatterns[i];
    }

    private async void StartPhase(FishSpawnPattern pattern)
    {
        _midPhase = true;
        var uniformSpawnRight = Random.value > 0.5f;
        
        foreach (var spawn in pattern.spawns)
        {
            if (spawn.repeatTimes < 1) spawn.repeatTimes = 1;
            int repeat = spawn.repeatTimes;
            while (repeat > 0)
            {
                if (_active == false)
                    return;

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

                var correctedAngle = -normalizedDir * spawn.angle + (normalizedDir == 1 ? 180 : 0);
                var rot = Quaternion.AngleAxis(correctedAngle, Vector3.forward);

                Instantiate(spawn.school, spawnPos, rot, transform);

                await UniTask.Delay(TimeSpan.FromSeconds(spawn.delayUntilNext * _difficultySettings.phaseInternalDelay.GetCurrent(_now)));

                repeat--;
            }
        }

        _midPhase = false;
        _lastPhaseEnd = Time.time;
    }
}
