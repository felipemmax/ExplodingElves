using System.Collections.Generic;
using ExplodingElves.Core.Characters;
using ExplodingElves.Interfaces;
using UnityEngine;

namespace ExplodingElves.Core.Services
{
    public class SpawnerRegistryService
    {
        private readonly Dictionary<ElfColor, ISpawner> _spawnersByColor = new();

        public SpawnerRegistryService(List<ISpawner> spawners)
        {
            if (spawners == null || spawners.Count == 0)
            {
                Debug.LogWarning("[SpawnerRegistry] No spawners provided during construction");
                return;
            }

            foreach (ISpawner spawner in spawners)
                if (spawner != null)
                    _spawnersByColor[spawner.ElfColor] = spawner;

            Debug.Log($"[SpawnerRegistry] Registered {_spawnersByColor.Count} spawners");
        }

        public ISpawner GetSpawnerForColor(ElfColor color)
        {
            _spawnersByColor.TryGetValue(color, out ISpawner spawner);
            return spawner;
        }

        public bool RequestSpawn(ElfColor color)
        {
            ISpawner spawner = GetSpawnerForColor(color);

            if (spawner == null)
            {
                Debug.LogWarning($"[SpawnerRegistry] No spawner found for color {color}");
                return false;
            }

            spawner.Spawn();
            return true;
        }
    }
}