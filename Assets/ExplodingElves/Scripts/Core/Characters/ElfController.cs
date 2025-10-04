using System;
using ExplodingElves.Core.Characters.States;
using ExplodingElves.Core.Collision;
using ExplodingElves.Interfaces;

namespace ExplodingElves.Core.Characters
{
    public class ElfController
    {
        private readonly IElfCollisionStrategy _collisionStrategy;
        private readonly ElfMovementBehavior _movement;
        private readonly ElfStateManager _stateManager;

        public ElfController(
            Elf elf,
            INavMeshAgentWrapper agent,
            IElfCollisionStrategy collisionStrategy)
        {
            Elf = elf ?? throw new ArgumentNullException(nameof(elf));
            if (agent == null) throw new ArgumentNullException(nameof(agent));

            _collisionStrategy = collisionStrategy ?? throw new ArgumentNullException(nameof(collisionStrategy));
            _movement = new ElfMovementBehavior(agent, Elf.Data.DirectionChangeInterval);
            _stateManager = new ElfStateManager(Elf, _movement);
        }

        public Elf Elf { get; }
        public bool IsDead => _stateManager.IsDead;
        public bool IsStunned => _stateManager.IsStunned;
        public bool IsMoving => _movement.GetCurrentSpeed() > 0.1f;

        public void Update(float deltaTime)
        {
            _stateManager.Update(deltaTime);
        }

        public void ApplyStun()
        {
            _stateManager.ApplyStun();
        }

        public void Kill()
        {
            _stateManager.Kill();
        }

        public CollisionDecision HandleCollisionWith(ElfController other)
        {
            if (other == null || IsDead || other.IsDead)
                return CollisionDecision.None();

            return _collisionStrategy.Decide(Elf, other.Elf);
        }
    }
}