using System;
using UnityEngine;

namespace Fish
{
    [Serializable]
    public class FishSpawn
    {
        public FishSpawnMode spawnMode;
        public FishSchool school;
        public float height;
        public float angle;
        public float delayUntilNext;
    }
}