using System.Collections.Generic;
using System.Linq;
using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Spawners;
using ExplodingElves.Interfaces;

namespace ExplodingElves.Core.Services
{
    public class SpawnerRegistryService : ISpawnerRegistryService
    {
        private readonly List<ISpawner> _spawners = new();

        public void Register(ISpawner spawner)
        {
            if (!_spawners.Contains(spawner)) _spawners.Add(spawner);
        }

        public void Unregister(ISpawner spawner)
        {
            _spawners.Remove(spawner);
        }

        public ISpawner GetSpawner(ElfColor color)
        {
            return _spawners.FirstOrDefault(s => s.ElfColor == color);
        }
    }
}