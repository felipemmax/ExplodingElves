using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Characters.Collision;

namespace ExplodingElves.Interfaces
{
    public interface IElfCollisionStrategy
    {
        CollisionDecision Decide(Elf elf1, Elf elf2);
    }
}