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

    [SerializeField] private GameObject _pufferSpawn;
    [SerializeField] private GameObject _sharkSpawn;

    private float _timeBetweenPhases = 8f;

    private bool _active = false;
    private float _lastPhaseStart = 0;
    private float _lastPufferSent = 0;
    private float _lastSharkSent = 0;
    private bool _midPhase = false;

    private GameManager gm => GameManager.Instance;

    private float _now;

    public void Begin()
    {
        _difficultySettings = gm.difficulty;
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

        _timeBetweenPhases = 1 / _difficultySettings.phaseRate.GetCurrent(_now);
        float timeBetweenPuffers = 1 / _difficultySettings.pufferRate.GetCurrent(_now);
        float timeBetweenSharks = 1 / _difficultySettings.sharkRate.GetCurrent(_now);

        if (Time.time - _lastPufferSent >= timeBetweenPuffers)
        {
            _lastPufferSent = Time.time;
            SendFish(_pufferSpawn);
        }

        if (Time.time - _lastSharkSent >= timeBetweenSharks)
        {
            _lastSharkSent = Time.time;
            SendFish(_sharkSpawn);
        }

        if (Time.time - _lastPhaseStart >= _timeBetweenPhases)
        {
            _lastPhaseStart = Time.time;
            StartPhase(GetRandomSpawnPattern());
        }

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
                spawnPos.Scale(gm.GetArenaBounds().extents);
                spawnPos += gm.GetArenaBounds().center;

                var correctedAngle = -normalizedDir * spawn.angle + (normalizedDir == 1 ? 180 : 0);
                var rot = Quaternion.AngleAxis(correctedAngle, Vector3.forward);

                Instantiate(spawn.school, spawnPos, rot, transform);

                await UniTask.Delay(TimeSpan.FromSeconds(spawn.delayUntilNext * _difficultySettings.phaseInternalDelay.GetCurrent(_now)));

                repeat--;
            }
        }

        _midPhase = false;
    }

    private void SendFish(GameObject gameObject)
    {
        float height = Random.value * 1.6f - 0.8f;
        float direction = Random.value > 0.5f ? 1 : -1;
        Vector3 spawnPos = new Vector3(direction * 1.2f, height);
        spawnPos.Scale(gm.GetArenaBounds().extents);
        spawnPos += gm.GetArenaBounds().center;

        float correctedAngle = direction == 1 ? 180 : 0;
        Quaternion rot = Quaternion.AngleAxis(correctedAngle, Vector3.forward);

        Instantiate(gameObject, spawnPos, rot, transform);
    }
}
