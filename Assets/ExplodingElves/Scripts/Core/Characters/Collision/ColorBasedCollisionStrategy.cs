using ExplodingElves.Interfaces;

namespace ExplodingElves.Core.Characters.Collision
{
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