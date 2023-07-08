using System;
using UnityEngine;

namespace Fish
{
    [Serializable]
    public class FishSpawn
    {
        public FishSpawnMode spawnMode;
        public GameObject school;
        public float height;
        public float angle;
        public float delayUntilNext;
        public int repeatTimes = 1;
    }
}