using ExplodingElves.Core.Characters;
using UnityEngine;

namespace ExplodingElves.Core.Spawners
{
    public class SpawnerData
    {
        public SpawnerData(float spawnRadius, ElfData elfData, GameObject elfPrefab, Vector3 spawnPosition)
        {
            SpawnRadius = spawnRadius;
            ElfData = elfData;
            ElfPrefab = elfPrefab;
            SpawnPosition = spawnPosition;
        }

        public float SpawnRadius { get; }
        public ElfData ElfData { get; }
        public GameObject ElfPrefab { get; }
        public Vector3 SpawnPosition { get; }
    }
}