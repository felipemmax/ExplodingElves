using System.Collections.Generic;
using ExplodingElves.Interfaces;
using UnityEngine;

namespace ExplodingElves.Core.Characters
{
    /// <summary>
    ///     Service to manage and locate spawners by color.
    ///     Single Responsibility: Registry of all spawners in the scene.
    /// </summary>
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

        /// <summary>
        ///     Get the spawner responsible for a specific color.
        /// </summary>
        public ISpawner GetSpawnerForColor(ElfColor color)
        {
            _spawnersByColor.TryGetValue(color, out ISpawner spawner);
            return spawner;
        }

        /// <summary>
        ///     Request a spawn from the appropriate spawner.
        /// </summary>
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