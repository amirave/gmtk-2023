using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fish
{
    [CreateAssetMenu]
    public class FishSpawnPattern : ScriptableObject
    {
        [SerializeField] public float weight;
        [SerializeField] public bool randomHeightOffset;
        [SerializeField] public List<FishSpawn> spawns;

        public float GetTotalTime() // TODO: make it work with repeated patterns
        {
            return spawns.Sum(s => s.delayUntilNext);
        }
    }
}