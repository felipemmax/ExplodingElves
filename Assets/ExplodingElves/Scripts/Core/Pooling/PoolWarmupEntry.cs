using System;
using UnityEngine;

namespace ExplodingElves.Core.Pooling
{
    [Serializable]
    public class PoolWarmupEntry
    {
        [Tooltip("The prefab to pre-instantiate in the pool")]
        public GameObject prefab;

        [Min(0)] [Tooltip("Number of instances to pre-instantiate")]
        public int warmupCount;
    }
}