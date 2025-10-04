using System.Collections.Generic;
using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Spawners;
using ExplodingElves.Interfaces;

namespace ExplodingElves.Tests.Mocks
{
    public class MockSpawnerRegistryService : ISpawnerRegistryService
    {
        private readonly Dictionary<ElfColor, ISpawner> _spawners = new Dictionary<ElfColor, ISpawner>();
        
        public int RegisterCallCount { get; private set; }
        public int UnregisterCallCount { get; private set; }
        public int GetSpawnerCallCount { get; private set; }

        public void Register(ISpawner spawner)
        {
            RegisterCallCount++;
            _spawners[spawner.ElfColor] = spawner;
        }

        public void Unregister(ISpawner spawner)
        {
            UnregisterCallCount++;
            _spawners.Remove(spawner.ElfColor);
        }

        public ISpawner GetSpawner(ElfColor color)
        {
            GetSpawnerCallCount++;
            return _spawners.TryGetValue(color, out var spawner) ? spawner : null;
        }
    }
}