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
}