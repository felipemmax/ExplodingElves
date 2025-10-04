using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Collision;
using ExplodingElves.Interfaces;

namespace ExplodingElves.Tests.Mocks
{
    public class MockElfCollisionStrategy : IElfCollisionStrategy
    {
        public CollisionDecision DecisionToReturn { get; set; } = CollisionDecision.None();
        public int DecideCallCount { get; private set; }
        public Elf LastElf1 { get; private set; }
        public Elf LastElf2 { get; private set; }

        public CollisionDecision Decide(Elf elf1, Elf elf2)
        {
            DecideCallCount++;
            LastElf1 = elf1;
            LastElf2 = elf2;
            return DecisionToReturn;
        }
    }
}