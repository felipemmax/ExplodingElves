using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Collision;

namespace ExplodingElves.Interfaces
{
    /// <summary>
    ///     Strategy pattern: Define how elves should react to collisions.
    ///     Open/Closed Principle: Add new collision rules without modifying existing code.
    /// </summary>
    public interface IElfCollisionStrategy
    {
        CollisionDecision Decide(Elf elf1, Elf elf2);
    }

    public class ColorBasedCollisionStrategy : IElfCollisionStrategy
    {
        public CollisionDecision Decide(Elf elf1, Elf elf2)
        {
            if (elf1 == null || elf2 == null)
                return CollisionDecision.None();

            if (elf1.Color == elf2.Color)
                return CollisionDecision.SpawnExtra();

            return CollisionDecision.ExplodeBoth();
        }
    }
}