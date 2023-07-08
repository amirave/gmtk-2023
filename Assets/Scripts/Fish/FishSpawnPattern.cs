using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fish
{
    [CreateAssetMenu]
    public class FishSpawnPattern : ScriptableObject
    {
        [SerializeField] public List<FishSpawn> spawns;

        public float GetTotalTime()
        {
            return spawns.Sum(s => s.delayUntilNext);
        }
    }
}