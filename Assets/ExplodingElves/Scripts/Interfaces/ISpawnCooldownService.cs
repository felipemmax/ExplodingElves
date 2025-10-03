namespace ExplodingElves.Interfaces
{
    public interface ISpawnCooldownService
    {
        bool CanSpawn();
        float GetRemainingCooldown();
        void RegisterSpawn();
    }
}