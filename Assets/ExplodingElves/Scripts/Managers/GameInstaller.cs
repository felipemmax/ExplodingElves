using ExplodingElves.Core;
using ExplodingElves.Interfaces;
using ExplodingElves.Pools;
using ExplodingElves.Services;
using ExplodingElves.Views;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Managers
{
    /// <summary>
    /// Dependency injection configuration for the game.
    /// Sets up all bindings using Zenject IoC container.
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        [Header("Prefab Resources")]
        [Tooltip("Resource path for the Elf prefab (Resources/Prefabs/Elf)")]
        public string elfPrefabResourcePath = "Prefabs/Elf";

        [Tooltip("Resource path for the explosion VFX prefab (Resources/Prefabs/ExplosionFX)")]
        public string explosionPrefabResourcePath = "Prefabs/ExplosionFX";
        
        [Header("Pool Settings")]
        [Min(0)] public int elfPoolInitialSize = 16;
        [Min(0)] public int explosionWarmupCount = 8;

        [Header("Gameplay Settings")]
        [Min(0f)] 
        [Tooltip("Cooldown in seconds between extra spawns from collisions")]
        public float extraSpawnCooldownSeconds = 1.5f;

        public override void InstallBindings()
        {
            BindCoreServices();
            BindPools();
            BindManagers();
            BindSceneComponents();
            
            WarmupPools();
            
            Debug.Log("[GameInstaller] All bindings installed successfully");
        }

        private void BindCoreServices()
        {
            // Core game logic services
            Container.Bind<CollisionHandler>().AsSingle();
            Container.Bind<SpawnCooldownService>().AsSingle()
                .WithArguments(extraSpawnCooldownSeconds);
            Container.Bind<ExplosionService>().AsSingle();
        }

        private void BindPools()
        {
            // Bind generic prefab pool first (required by other services)
            Container.Bind<IPrefabPool>().To<PrefabPool>().AsSingle();

            // Bind specialized elf pool
            Container.BindMemoryPool<ElfView, ElfPool>()
                .WithInitialSize(elfPoolInitialSize)
                .FromComponentInNewPrefabResource(elfPrefabResourcePath)
                .UnderTransformGroup("Elves");
        }

        private void BindManagers()
        {
            Container.Bind<SpawnManager>().AsSingle();
            Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        }

        private void BindSceneComponents()
        {
            // Auto-discover all spawners in scene
            Container.Bind<ISpawner>().FromComponentsInHierarchy().AsCached();
        }

        private void WarmupPools()
        {
            // Pre-instantiate explosion effects to avoid runtime hiccups
            var explosionPrefab = Resources.Load<GameObject>(explosionPrefabResourcePath);
            
            if (explosionPrefab != null && explosionWarmupCount > 0)
            {
                var prefabPool = Container.Resolve<IPrefabPool>();
                prefabPool.Warmup(explosionPrefab, explosionWarmupCount);
                Debug.Log($"[GameInstaller] Warmed up {explosionWarmupCount} explosion instances");
            }
            else if (explosionPrefab == null)
            {
                Debug.LogWarning($"[GameInstaller] Explosion prefab not found at '{explosionPrefabResourcePath}'");
            }
        }
    }
}