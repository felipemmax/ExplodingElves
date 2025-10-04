using System;
using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Spawners;

namespace ExplodingElves.Tests.Mocks
{
    public class MockSpawner : ISpawner
    {
        public int SpawnCallCount { get; private set; }
        public int StartCallCount { get; private set; }
        public int StopCallCount { get; private set; }
        public int ChangeSpawnRateCallCount { get; private set; }
        public ElfColor ElfColor { get; set; }
        public string SpawnRateDisplay { get; set; } = "Test";

        public event Action OnStateChanged;

        public void Start()
        {
            StartCallCount++;
        }

        public void Stop()
        {
            StopCallCount++;
        }

        public void ChangeSpawnRate()
        {
            ChangeSpawnRateCallCount++;
            OnStateChanged?.Invoke();
        }

        public void Spawn()
        {
            SpawnCallCount++;
        }

        public void Dispose()
        {
        }

        public void TriggerStateChanged()
        {
            OnStateChanged?.Invoke();
        }
    }
}