using ExplodingElves.Core.Pooling;
using UnityEngine;

namespace ExplodingElves.Infrastructure.Configuration
{
    [CreateAssetMenu(fileName = "PoolWarmupConfig", menuName = "ExplodingElves/Pool Warmup Configuration")]
    public class PoolWarmupConfig : ScriptableObject
    {
        [Tooltip("List of prefabs to warmup with their respective counts")]
        public PoolWarmupEntry[] warmupEntries = new PoolWarmupEntry[0];
    }
}