using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu()]
public class DifficultySettings : ScriptableObject
{
    public Difficulty phaseDifficulty;
    public Difficulty phaseRate;
    public Difficulty maxFishAllowed;
    public Difficulty phaseInternalDelay;
    public Difficulty fishSpeed;
    public Difficulty pufferRate;
    public Difficulty sharkRate;
}

[Serializable]
public struct Difficulty
{
    public float min;
    public float max;
    public float timeUntilMax;
    public bool unclamped;

    public float GetCurrent(float time)
    {
        return unclamped ? Mathf.LerpUnclamped(min, max, time / timeUntilMax) : Mathf.Lerp(min, max, time / timeUntilMax);
    }
}
