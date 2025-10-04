using ExplodingElves.Core.Characters;
using ExplodingElves.Interfaces;

namespace ExplodingElves.Core.Collision
{
    public class ElfCollisionHandler
    {
        private readonly ISpawnCooldownService _cooldownService;
        private readonly ISpawnRequestService _spawnRequestService;

        public ElfCollisionHandler(
            ISpawnCooldownService cooldownService,
            ISpawnRequestService spawnRequestService)
        {
            _cooldownService = cooldownService;
            _spawnRequestService = spawnRequestService;
        }

        public CollisionResult ProcessCollision(
            ElfController controller1,
            ElfController controller2)
        {
            // Apply immediate effects (could be an interface if needed)
            controller1.ApplyStun();
            controller2.ApplyStun();

            // Delegate business decision to domain object
            CollisionDecision decision = controller1.HandleCollisionWith(controller2);

            CollisionResult result = new()
            {
                Decision = decision,
                ShouldExplodeBoth = decision.ShouldExplodeBoth,
                ShouldSpawnExtra = false
            };

            if (decision.ShouldSpawnExtra) result.ShouldSpawnExtra = TryRequestSpawn(controller1.Elf.Color);

            return result;
        }

        private bool TryRequestSpawn(ElfColor color)
        {
            if (_cooldownService == null || _spawnRequestService == null) return false;

            if (!_cooldownService.CanSpawn())
            {
                return false;
            }

            bool spawned = _spawnRequestService.RequestSpawn(color);

            if (spawned) _cooldownService.RegisterSpawn();

            return spawned;
        }

        public struct CollisionResult
        {
            public CollisionDecision Decision;
            public bool ShouldExplodeBoth;
            public bool ShouldSpawnExtra;
        }
    }
}