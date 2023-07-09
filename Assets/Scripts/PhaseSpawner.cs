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
using Audio;

public class PhaseSpawner : MonoBehaviour
{
    private DifficultySettings _difficultySettings;
    [SerializeField] private List<FishSpawnPattern> _easySpawnPatterns;
    [SerializeField] private List<FishSpawnPattern> _medSpawnPatterns;
    [SerializeField] private List<FishSpawnPattern> _hardSpawnPatterns;

    private List<FishSpawnPattern> _possiblePatterns;
    private int _currentDifficulty = 0;

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
        _possiblePatterns = _easySpawnPatterns;

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

        int patternDifficulty = (int)(_difficultySettings.phaseDifficulty.GetCurrent(_now) * 3);

        if (patternDifficulty > _currentDifficulty )
        {
            if (patternDifficulty == 1)
                _possiblePatterns.AddRange(_medSpawnPatterns);
            else if (patternDifficulty == 2)
                _possiblePatterns.AddRange(_hardSpawnPatterns);

            _currentDifficulty = patternDifficulty;
        }

        _timeBetweenPhases = 1 / _difficultySettings.phaseRate.GetCurrent(_now);
        float timeBetweenPuffers = 1 / _difficultySettings.pufferRate.GetCurrent(_now - _difficultySettings.pufferSpawnStart);
        float timeBetweenSharks = 1 / _difficultySettings.sharkRate.GetCurrent(_now - _difficultySettings.sharkSpawnStart);

        if (_now - _difficultySettings.pufferSpawnStart - _lastPufferSent >= timeBetweenPuffers)
        {
            _lastPufferSent = Time.time;
            SendFish(_pufferSpawn);
            AudioManager.Instance.PlayEffect("pufferfish_enter");
        }

        if (_now - _difficultySettings.sharkSpawnStart - _lastSharkSent >= timeBetweenSharks)
        {
            _lastSharkSent = Time.time;
            SendFish(_sharkSpawn);
            AudioManager.Instance.PlayEffect("shark_enter");
        }
        
        if (GameManager.Instance.player._idleTime > 5f)
        {
            _lastSharkSent = Time.time;
            SendFish(_sharkSpawn);
            AudioManager.Instance.PlayEffect("shark_enter");
            GameManager.Instance.player._idleTime = 0;
        }

        if (FindObjectsOfType<FishAI>().Length > _difficultySettings.maxFishAllowed.GetCurrent(_now))
            return;

        if (Time.time - _lastPhaseStart >= _timeBetweenPhases)
        {
            _lastPhaseStart = Time.time;
            StartPhase(GetRandomSpawnPattern());
        }

        _now += Time.deltaTime;
    }

    private FishSpawnPattern GetRandomSpawnPattern()
    {
        return _possiblePatterns[Utils.GetRandomWeightedIndex(_possiblePatterns.Select(p => p.weight).ToArray())];
    }

    private async void StartPhase(FishSpawnPattern pattern)
    {
        _midPhase = true;
        var uniformSpawnRight = Random.value > 0.5f;
        float heightOffset = 0;
        if (pattern.randomHeightOffset)
            heightOffset = Random.value * 1f - 0.5f;


        foreach (var spawn in pattern.spawns)
        {
            if (spawn.repeatTimes < 1) spawn.repeatTimes = 1;
            int repeat = spawn.repeatTimes;
            while (repeat > 0)
            {
                if (_active == false)
                    return;
                
                var normalizedDir = spawn.spawnMode switch
                {
                    FishSpawnMode.Left => uniformSpawnRight ? 1 : -1,
                    FishSpawnMode.Right => uniformSpawnRight ? -1 : 1,
                    FishSpawnMode.Random => Random.value > 0.5f ? -1 : 1
                };

                var spawnPos = new Vector3(normalizedDir * 1.2f, spawn.height + heightOffset);
                spawnPos.Scale(gm.GetArenaBounds().extents);
                spawnPos += gm.GetArenaBounds().center;

                var usedAngle = spawn.angle;
                if (usedAngle == -100)
                    usedAngle = Random.value * 30 - 15;
                var correctedAngle = -normalizedDir * usedAngle + (normalizedDir == 1 ? 180 : 0);
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
        float height = Mathf.Lerp(0, 0.8f, Random.value);
        float direction = Random.value > 0.5f ? 1 : -1;
        Vector3 spawnPos = new Vector3(direction * 1.2f, height);
        spawnPos.Scale(gm.GetArenaBounds().extents);
        spawnPos += gm.GetArenaBounds().center;

        float correctedAngle = direction == 1 ? 180 : 0;
        Quaternion rot = Quaternion.AngleAxis(correctedAngle, Vector3.forward);

        Instantiate(gameObject, spawnPos, rot, transform);
    }
}
