using System;
using ExplodingElves.Core.Characters;

namespace ExplodingElves.Core.Spawners
{
    public interface ISpawner : IDisposable
    {
        string SpawnRateDisplay { get; }
        ElfColor ElfColor { get; }
        event Action OnStateChanged;
        void Start();
        void Stop();
        void ChangeSpawnRate();
        void Spawn();
    }
}