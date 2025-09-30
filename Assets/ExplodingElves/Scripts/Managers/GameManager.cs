using System.Collections.Generic;
using ExplodingElves.Interfaces;
using ExplodingElves.Pools;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Tooltip("Spawn one elf from each spawner on start.")] [SerializeField]
        private bool spawnOnStart = true;

        private ElfPool _pool;
        private List<ISpawner> _spawners;

        private void Start()
        {
            if (!spawnOnStart || _spawners == null) return;
            foreach (ISpawner s in _spawners) s?.Spawn();
        }

        [Inject]
        private void Construct(List<ISpawner> spawners, ElfPool pool)
        {
            _spawners = spawners;
            _pool = pool;
        }
    }
}