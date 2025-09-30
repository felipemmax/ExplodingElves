namespace ExplodingElves.Core
{
    /// <summary>
    ///     Pure business logic for resolving collision outcomes between elves.
    ///     SRP: Only determines collision results, doesn't execute them.
    ///     OCP: Can be extended for new collision rules without modifying existing code.
    /// </summary>
    public class CollisionHandler
    {
        /// <summary>
        ///     Determines outcome when two elves of the same color collide.
        ///     Rule: Same color spawns an extra elf.
        /// </summary>
        public CollisionDecision HandleSameColor(ElfLogic elf1, ElfLogic elf2)
        {
            // Future: Could add logic based on elf states, velocities, etc.
            return CollisionDecision.SpawnExtra();
        }

        /// <summary>
        ///     Determines outcome when two elves of different colors collide.
        ///     Rule: Different colors cause both to explode.
        /// </summary>
        public CollisionDecision HandleDifferentColor(ElfLogic elf1, ElfLogic elf2)
        {
            // Future: Could add logic for partial explosions, score tracking, etc.
            return CollisionDecision.ExplodeBoth();
        }
    }
}