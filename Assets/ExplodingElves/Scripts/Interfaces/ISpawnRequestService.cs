using ExplodingElves.Core.Characters;

namespace ExplodingElves.Interfaces
{
    public interface ISpawnRequestService
    {
        bool RequestSpawn(ElfColor color);
    }
}