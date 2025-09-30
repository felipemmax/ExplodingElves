using ExplodingElves.Core;
using ExplodingElves.Pools;
using ExplodingElves.Services;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Managers
{
    public class SpawnManager
    {
        private readonly SpawnCooldownService _cooldown;
        private readonly ElfPool _pool;

        [Inject]
        public SpawnManager(ElfPool pool, SpawnCooldownService cooldown)
        {
            _pool = pool;
            _cooldown = cooldown;
        }

        public void SpawnExtra(ElfData data, Vector3 position)
        {
            if (_cooldown != null && !_cooldown.TryRequest(data.ElfColor))
                return;

            _pool.Spawn(position, data);
        }
    }
}