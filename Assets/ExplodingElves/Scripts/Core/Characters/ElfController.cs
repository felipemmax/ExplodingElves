using System;
using ExplodingElves.Core.Collision;
using ExplodingElves.Interfaces;
using UnityEngine.AI;

namespace ExplodingElves.Core.Characters
{
    public class ElfController
    {
        private readonly IElfCollisionStrategy _collisionStrategy;
        private readonly ElfMovementBehavior _movement;
        private readonly ElfStateManager _stateManager;

        public ElfController(
            Elf initialState,
            NavMeshAgent agent,
            IElfCollisionStrategy collisionStrategy)
        {
            if (initialState == null) throw new ArgumentNullException(nameof(initialState));
            if (agent == null) throw new ArgumentNullException(nameof(agent));

            _stateManager = new ElfStateManager(initialState);
            _collisionStrategy = collisionStrategy ?? throw new ArgumentNullException(nameof(collisionStrategy));
            _movement = new ElfMovementBehavior(agent, initialState.Data.DirectionChangeInterval);
        }

        public Elf CurrentState => _stateManager.CurrentState;
        public bool IsMoving => _movement.GetCurrentSpeed() > 0.1f;

        public void Update(float deltaTime)
        {
            if (_stateManager.IsDead)
                return;

            bool wasStunned = _stateManager.IsStunned;
            _stateManager.Update(deltaTime);
            bool isStunnedNow = _stateManager.IsStunned;

            // Resume movement when stun ends
            if (wasStunned && !isStunnedNow)
                _movement.Resume();

            _movement.Update(deltaTime, isStunnedNow, _stateManager.IsDead);
        }

        public void ApplyStun()
        {
            _stateManager.ApplyStun();
            _movement.Stop();
        }

        public void Kill()
        {
            _stateManager.Kill();
            _movement.Stop();
        }

        public CollisionDecision HandleCollisionWith(ElfController other)
        {
            if (other == null || _stateManager.IsDead || other._stateManager.IsDead)
                return CollisionDecision.None();

            return _collisionStrategy.Decide(CurrentState, other.CurrentState);
        }
    }
}