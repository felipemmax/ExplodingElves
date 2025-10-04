using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Spawners;

namespace ExplodingElves.Interfaces
{
    public interface ISpawnerRegistryService
    {
        void Register(ISpawner spawner);
        void Unregister(ISpawner spawner);
        ISpawner GetSpawner(ElfColor color);
    }
}