using ExplodingElves.Core.Characters;
using ExplodingElves.Interfaces;

namespace ExplodingElves.Tests.Mocks
{
    public class MockSpawnRequestService : ISpawnRequestService
    {
        public bool RequestSpawnResult { get; set; } = true;
        public ElfColor LastRequestedColor { get; private set; }
        public int RequestSpawnCallCount { get; private set; }

        public bool RequestSpawn(ElfColor color)
        {
            RequestSpawnCallCount++;
            LastRequestedColor = color;
            return RequestSpawnResult;
        }
    }
}