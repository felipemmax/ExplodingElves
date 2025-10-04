using ExplodingElves.Core.Characters.Collision;
using ExplodingElves.Core.Characters.Services;
using ExplodingElves.Core.Pooling;
using ExplodingElves.Core.Services;
using ExplodingElves.Core.Spawners;
using ExplodingElves.Core.Spawners.Services;
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

            if (_poolWarmupConfig != null)
            {
                Container.Bind<PoolWarmupConfig>().FromInstance(_poolWarmupConfig).AsSingle();
                Container.BindInterfacesAndSelfTo<PoolWarmer>().AsSingle();
            }
            else
            {
                Debug.LogWarning("[GameInstaller] No PoolWarmupConfig assigned. Skipping pool warmup.");
            }

            Debug.Log("[GameInstaller] All bindings installed successfully");
        }

        private void BindCoreServices()
        {
            // Core game logic services
            Container.Bind<IVFXService>().To<VFXService>().AsSingle();

            // Collision strategy - using color-based rules
            Container.Bind<IElfCollisionStrategy>().To<ColorBasedCollisionStrategy>().AsSingle();

            // Spawn services
            Container.Bind<ISpawnCooldownService>().To<SpawnGlobalCooldownService>()
                .AsSingle()
                .WithArguments(extraSpawnCooldownSeconds);
            Container.Bind<ISpawnRequestService>().To<SpawnRequestService>().AsSingle();

            Container.Bind<ElfCollisionHandler>()
                .AsSingle()
                .NonLazy();

            // Spawner services and factory
            Container.Bind<ISpawnerRegistryService>().To<SpawnerRegistryService>().AsSingle();
            // 1. Tell Zenject how to create an ISpawner
            Container.Bind<ISpawner>().To<Spawner>().AsTransient();
            // This single line creates the factory implementation for us.
            // It binds IFactory<SpawnerData, ISpawner> to an auto-generated factory
            // that creates instances of the Spawner class.
            Container.BindIFactory<SpawnerData, ISpawner>().To<Spawner>().AsSingle();
            Container.Bind<IGameObjectInstantiator>().To<ZenjectInstantiator>().AsSingle();
        }

        private void BindPools()
        {
            Container.Bind<IPrefabPool>().To<PrefabPool>().AsSingle();
        }
    }
}