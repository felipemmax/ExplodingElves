using System;
using ExplodingElves.Core.Characters;
using ExplodingElves.Interfaces;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace ExplodingElves.Core.Spawners
{
    public class Spawner : ISpawner, ITickable
    {
        private readonly SpawnerData _data;
        private readonly IPrefabPool _pool;
        private readonly ISpawnerRegistryService _registryService;
        private readonly float[] _spawnRates = { 0f, 3f, 6f, 12f, 24f, 48f };
        private readonly TickableManager _tickableManager;
        private int _currentSpawnRateIndex = 3;
        private bool _isSpawning;
        private float _spawnTimer;

        public Spawner(SpawnerData data, IPrefabPool pool, TickableManager tickableManager,
            ISpawnerRegistryService registryService)
        {
            _data = data;
            _pool = pool;
            _tickableManager = tickableManager;
            _registryService = registryService;

            _tickableManager.Add(this);
            _registryService.Register(this);
            ResetTimer();
        }

        public event Action OnStateChanged;

        public string SpawnRateDisplay
        {
            get
            {
                float currentCooldown = _spawnRates[_currentSpawnRateIndex];
                return currentCooldown > 0 ? $"{currentCooldown}s" : "Disabled";
            }
        }

        public ElfColor ElfColor => _data.ElfData.ElfColor;

        public void Start()
        {
            _isSpawning = true;
            ResetTimer();
            Spawn();
        }

        public void Stop()
        {
            _isSpawning = false;
        }

        public void ChangeSpawnRate()
        {
            _currentSpawnRateIndex = (_currentSpawnRateIndex + 1) % _spawnRates.Length;
            ResetTimer();
            OnStateChanged?.Invoke();
        }

        public void Dispose()
        {
            _tickableManager.Remove(this);
            _registryService.Unregister(this);
        }

        public void Spawn()
        {
            Vector3 spawnPos = _data.SpawnPosition + Random.insideUnitSphere * _data.SpawnRadius;
            spawnPos.y = _data.SpawnPosition.y;

            GameObject spawnedElf = _pool.Spawn(_data.ElfPrefab, spawnPos, Quaternion.identity);

            if (spawnedElf != null)
            {
                if (spawnedElf.TryGetComponent(out ElfView elfView))
                    elfView.OnSpawned(spawnPos, _data.ElfData);
                else
                    Debug.LogError("Spawned prefab does not have ElfView component!");
            }
        }

        public void Tick()
        {
            if (!_isSpawning)
                return;

            float currentCooldown = _spawnRates[_currentSpawnRateIndex];
            if (currentCooldown <= 0) // A cooldown of 0 or less means no spawning
                return;

            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0)
            {
                Spawn();
                ResetTimerForNextSpawn();
            }
        }

        private void ResetTimer()
        {
            float cooldown = _spawnRates[_currentSpawnRateIndex];
            _spawnTimer = cooldown > 0 ? cooldown : float.MaxValue; // Use cooldown directly; 0 means infinite cooldown
        }

        private void ResetTimerForNextSpawn()
        {
            float cooldown = _spawnRates[_currentSpawnRateIndex];

            if (cooldown > 0) _spawnTimer += cooldown;
        }
    }
}