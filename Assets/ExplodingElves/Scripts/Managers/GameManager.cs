using System.Collections.Generic;
using ExplodingElves.Core;
using ExplodingElves.Interfaces;
using ExplodingElves.Pools;
using ExplodingElves.Views;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Tooltip("Spawn one elf from each spawner on start.")] [SerializeField]
        private bool spawnOnStart = true;
        private List<ISpawner> _spawners;

        private void Start()
        {
            if (!spawnOnStart || _spawners == null) return;
            foreach (ISpawner spawner in _spawners) spawner?.Spawn();
        }

        [Inject]
        private void Construct(List<ISpawner> spawners)
        {
            _spawners = spawners;
        }
    }
}