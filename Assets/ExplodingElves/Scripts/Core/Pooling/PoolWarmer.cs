using ExplodingElves.Infrastructure.Configuration;
using ExplodingElves.Interfaces;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Core.Pooling
{
    public class PoolWarmer : IInitializable
    {
        private readonly PoolWarmupConfig _poolWarmupConfig;
        private readonly IPrefabPool _prefabPool;

        public PoolWarmer(IPrefabPool prefabPool, PoolWarmupConfig poolWarmupConfig)
        {
            _prefabPool = prefabPool;
            _poolWarmupConfig = poolWarmupConfig;
        }

        public void Initialize()
        {
            foreach (PoolWarmupEntry entry in _poolWarmupConfig.warmupEntries)
            {
                if (entry.prefab == null)
                {
                    Debug.LogWarning("[PoolWarmer] Null prefab found in warmup configuration. Skipping.");
                    continue;
                }

                if (entry.warmupCount <= 0) continue;

                _prefabPool.Warmup(entry.prefab, entry.warmupCount);
                Debug.Log($"[PoolWarmer] Warmed up {entry.warmupCount} instances of '{entry.prefab.name}'");
            }
        }
    }
}