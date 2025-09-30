using ExplodingElves.Interfaces;
using ExplodingElves.Pools;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Services
{
    /// <summary>
    /// Service responsible for spawning explosion VFX.
    /// SRP: Only handles explosion visual effects.
    /// </summary>
    public class ExplosionService
    {
        private const float ExplosionLifetime = 2f;
        private static GameObject _explosionPrefab;
        
        private readonly IPrefabPool _prefabPool;
        private readonly DiContainer _container;

        [Inject]
        public ExplosionService(IPrefabPool prefabPool, DiContainer container)
        {
            _prefabPool = prefabPool;
            _container = container;
        }

        public void PlayExplosion(Vector3 position)
        {
            if (_explosionPrefab == null)
            {
                _explosionPrefab = Resources.Load<GameObject>("Prefabs/ExplosionFX");
                if (_explosionPrefab == null)
                {
                    Debug.LogWarning("ExplosionFX prefab not found at Resources/Prefabs/ExplosionFX");
                    return;
                }
            }

            if (_prefabPool == null)
            {
                // Fallback to direct instantiate
                GameObject fallback = _container.InstantiatePrefab(_explosionPrefab, position, Quaternion.identity, null);
                Object.Destroy(fallback, ExplosionLifetime);
                return;
            }

            GameObject go = _prefabPool.Spawn(_explosionPrefab, position, Quaternion.identity);

            if (!go.TryGetComponent<AutoReturnToPool>(out var auto))
                auto = go.AddComponent<AutoReturnToPool>();

            auto.Begin(ExplosionLifetime, _prefabPool);
        }
    }
}