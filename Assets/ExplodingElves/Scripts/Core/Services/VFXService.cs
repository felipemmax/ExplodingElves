using ExplodingElves.Core.Pooling;
using ExplodingElves.Interfaces;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Core.Services
{
    public class VFXService : IVFXService
    {
        private const float DefaultVFXLifetime = 2f;
        private readonly DiContainer _container;

        private readonly IPrefabPool _prefabPool;
        private GameObject _collisionVFXPrefab;

        private GameObject _explosionVFXPrefab;
        private GameObject _spawnVFXPrefab;

        [Inject]
        public VFXService(IPrefabPool prefabPool, DiContainer container)
        {
            _prefabPool = prefabPool;
            _container = container;
        }

        /// <summary>
        ///     Play explosion VFX when an elf dies.
        /// </summary>
        public void PlayExplosion(Vector3 position)
        {
            if (_explosionVFXPrefab == null)
            {
                _explosionVFXPrefab = Resources.Load<GameObject>("Prefabs/ExplosionFX");
                if (_explosionVFXPrefab == null)
                {
                    Debug.LogWarning("ExplosionFX prefab not found at Resources/Prefabs/ExplosionFX");
                    return;
                }
            }

            SpawnVFX(_explosionVFXPrefab, position, DefaultVFXLifetime);
        }

        /// <summary>
        ///     Play VFX when an elf spawns.
        /// </summary>
        public void PlaySpawnVFX(Vector3 position)
        {
            if (_spawnVFXPrefab == null)
            {
                _spawnVFXPrefab = Resources.Load<GameObject>("Prefabs/SpawnVFX");
                if (_spawnVFXPrefab == null)
                {
                    Debug.LogWarning("SpawnVFX prefab not found at Resources/Prefabs/SpawnVFX");
                    return;
                }
            }

            SpawnVFX(_spawnVFXPrefab, position, DefaultVFXLifetime);
        }

        /// <summary>
        ///     Play VFX when two elves of the same color collide.
        /// </summary>
        public void PlayCollisionVFX(Vector3 position)
        {
            if (_collisionVFXPrefab == null)
            {
                _collisionVFXPrefab = Resources.Load<GameObject>("Prefabs/CollisionVFX");
                if (_collisionVFXPrefab == null)
                {
                    Debug.LogWarning("CollisionVFX prefab not found at Resources/Prefabs/CollisionVFX");
                    return;
                }
            }

            SpawnVFX(_collisionVFXPrefab, position, DefaultVFXLifetime);
        }

        private void SpawnVFX(GameObject prefab, Vector3 position, float lifetime)
        {
            if (_prefabPool == null)
            {
                // Fallback to direct instantiate
                GameObject fallback = _container.InstantiatePrefab(prefab, position, Quaternion.identity, null);
                Object.Destroy(fallback, lifetime);
                return;
            }

            GameObject go = _prefabPool.Spawn(prefab, position, Quaternion.identity);

            if (!go.TryGetComponent(out AutoReturnToPool auto))
                auto = go.AddComponent<AutoReturnToPool>();

            auto.Begin(lifetime, _prefabPool);
        }
    }
}