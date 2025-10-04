using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Spawners;
using ExplodingElves.Interfaces;
using UnityEngine;

namespace ExplodingElves.Core.Services
{
    public class SpawnRequestService : ISpawnRequestService
    {
        private readonly ISpawnerRegistryService _spawnerRegistry;

        public SpawnRequestService(ISpawnerRegistryService spawnerRegistry)
        {
            _spawnerRegistry = spawnerRegistry;
        }

        public bool RequestSpawn(ElfColor color)
        {
            ISpawner spawner = _spawnerRegistry.GetSpawner(color);

            if (spawner != null)
            {
                spawner.Spawn();
                return true;
            }

            Debug.LogWarning($"[SpawnRequestService] No spawner found for color {color}.");
            return false;
        }
    }
}