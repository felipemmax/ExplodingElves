using ExplodingElves.Interfaces;

namespace ExplodingElves.Tests.Mocks
{
    public class MockSpawnCooldownService : ISpawnCooldownService
    {
        public bool CanSpawnResult { get; set; } = true;
        public float RemainingCooldown { get; set; } = 0f;
        
        public int CanSpawnCallCount { get; private set; }
        public int RegisterSpawnCallCount { get; private set; }
        public int GetRemainingCooldownCallCount { get; private set; }

        public bool CanSpawn()
        {
            CanSpawnCallCount++;
            return CanSpawnResult;
        }

        public float GetRemainingCooldown()
        {
            GetRemainingCooldownCallCount++;
            return RemainingCooldown;
        }

        public void RegisterSpawn()
        {
            RegisterSpawnCallCount++;
        }
    }
}