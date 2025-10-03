using System.Collections.Generic;
using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Collision;
using ExplodingElves.Core.Pooling;
using ExplodingElves.Core.Services;
using ExplodingElves.Infrastructure.Configuration;
using ExplodingElves.Interfaces;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Infrastructure.DI
{
    /// <summary>
    ///     Dependency injection configuration for the game.
    ///     Sets up all bindings using Zenject IoC container.
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        [Header("Pool Configuration")] [SerializeField]
        private PoolWarmupConfig _poolWarmupConfig;

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
            Container.Bind<IVFXService>().To<VFXService>().AsSingle();

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

            Container.Bind<ElfCollisionHandler>()
                .AsSingle()
                .NonLazy();
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
            if (_poolWarmupConfig == null)
            {
                Debug.LogWarning("[GameInstaller] No PoolWarmupConfig assigned. Skipping pool warmup.");
                return;
            }

            IPrefabPool prefabPool = Container.Resolve<IPrefabPool>();

            foreach (PoolWarmupEntry entry in _poolWarmupConfig.warmupEntries)
            {
                if (entry.prefab == null)
                {
                    Debug.LogWarning("[GameInstaller] Null prefab found in warmup configuration. Skipping.");
                    continue;
                }

                if (entry.warmupCount <= 0) continue;

                prefabPool.Warmup(entry.prefab, entry.warmupCount);
                Debug.Log($"[GameInstaller] Warmed up {entry.warmupCount} instances of '{entry.prefab.name}'");
            }
        }
    }
}