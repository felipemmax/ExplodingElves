using System.Collections.Generic;
using ExplodingElves.Core.Characters;
using ExplodingElves.Interfaces;
using ExplodingElves.Pools;
using ExplodingElves.Services;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Managers
{
    /// <summary>
    ///     Dependency injection configuration for the game.
    ///     Sets up all bindings using Zenject IoC container.
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        [Header("Prefabs")] [SerializeField] private GameObject elfPrefab;

        [Tooltip("Resource path for the explosion VFX prefab (Resources/Prefabs/ExplosionFX)")]
        public string explosionPrefabResourcePath = "Prefabs/ExplosionFX";

        [Tooltip("Resource path for the spawn VFX prefab (Resources/Prefabs/SpawnVFX)")]
        public string spawnVFXPrefabResourcePath = "Prefabs/SpawnVFX";

        [Tooltip("Resource path for the collision VFX prefab (Resources/Prefabs/CollisionVFX)")]
        public string collisionVFXPrefabResourcePath = "Prefabs/CollisionVFX";

        [Min(0)] public int explosionWarmupCount = 8;
        [Min(0)] public int spawnVFXWarmupCount = 5;
        [Min(0)] public int collisionVFXWarmupCount = 5;

        [Header("Gameplay Settings")] [Min(0f)] [Tooltip("Cooldown in seconds between extra spawns from collisions")]
        public float extraSpawnCooldownSeconds = 3f;

        public override void InstallBindings()
        {
            BindCoreServices();
            BindPools();
            BindSceneComponents();

            WarmupPools();

            Debug.Log("[GameInstaller] All bindings installed successfully");
        }

        private void BindCoreServices()
        {
            // Core game logic services
            Container.Bind<VFXService>().AsSingle();

            // Collision strategy - using color-based rules
            Container.Bind<IElfCollisionStrategy>().To<ColorBasedCollisionStrategy>().AsSingle();

            // Spawn cooldown service - GLOBAL cooldown
            Container.Bind<ElfSpawnCooldownService>()
                .AsSingle()
                .WithArguments(extraSpawnCooldownSeconds);

            // Spawner registry
            Container.Bind<SpawnerRegistryService>()
                .AsSingle()
                .OnInstantiated<SpawnerRegistryService>((context, registry) =>
                {
                    // Inject all spawners from the scene
                    List<ISpawner> spawners = context.Container.Resolve<List<ISpawner>>();
                    Debug.Log($"[GameInstaller] Resolved {spawners.Count} spawners for registry");
                });
        }

        private void BindPools()
        {
            Container.Bind<IPrefabPool>().To<PrefabPool>().AsSingle();
        }

        private void BindSceneComponents()
        {
            // Auto-discover all spawners in scene
            Container.Bind<ISpawner>().FromComponentsInHierarchy().AsCached();
        }

        private void WarmupPools()
        {
            IPrefabPool prefabPool = Container.Resolve<IPrefabPool>();

            // Pre-instantiate explosion effects to avoid runtime hiccups
            GameObject explosionPrefab = Resources.Load<GameObject>(explosionPrefabResourcePath);
            if (explosionPrefab != null && explosionWarmupCount > 0)
            {
                prefabPool.Warmup(explosionPrefab, explosionWarmupCount);
                Debug.Log($"[GameInstaller] Warmed up {explosionWarmupCount} explosion instances");
            }
            else if (explosionPrefab == null)
            {
                Debug.LogWarning($"[GameInstaller] Explosion prefab not found at '{explosionPrefabResourcePath}'");
            }

            // Pre-instantiate spawn VFX
            GameObject spawnVFXPrefab = Resources.Load<GameObject>(spawnVFXPrefabResourcePath);
            if (spawnVFXPrefab != null && spawnVFXWarmupCount > 0)
            {
                prefabPool.Warmup(spawnVFXPrefab, spawnVFXWarmupCount);
                Debug.Log($"[GameInstaller] Warmed up {spawnVFXWarmupCount} spawn VFX instances");
            }

            // Pre-instantiate collision VFX
            GameObject collisionVFXPrefab = Resources.Load<GameObject>(collisionVFXPrefabResourcePath);
            if (collisionVFXPrefab != null && collisionVFXWarmupCount > 0)
            {
                prefabPool.Warmup(collisionVFXPrefab, collisionVFXWarmupCount);
                Debug.Log($"[GameInstaller] Warmed up {collisionVFXWarmupCount} collision VFX instances");
            }
        }
    }
}