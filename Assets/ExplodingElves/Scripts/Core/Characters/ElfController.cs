using System;
using UnityEngine.AI;

namespace ExplodingElves.Core.Characters
{
    public class ElfController
    {
        private const float StunDuration = 0.5f;
        private readonly IElfCollisionStrategy _collisionStrategy;

        private readonly ElfMovementBehavior _movement;
        private float _stunTimeRemaining;

        public ElfController(Elf elf, NavMeshAgent agent, IElfCollisionStrategy collisionStrategy)
        {
            Model = elf ?? throw new ArgumentNullException(nameof(elf));
            _collisionStrategy = collisionStrategy ?? throw new ArgumentNullException(nameof(collisionStrategy));
            _movement = new ElfMovementBehavior(agent, elf.Data.DirectionChangeInterval);
        }

        public Elf Model { get; }

        public bool IsMoving => _movement.GetCurrentSpeed() > 0.1f;

        public void Update(float deltaTime)
        {
            if (Model.IsDead)
                return;

            UpdateStun(deltaTime);
            _movement.Update(deltaTime, Model.IsStunned, Model.IsDead);
        }

        public void ApplyStun()
        {
            Model.Stun();
            _stunTimeRemaining = StunDuration;
            _movement.Stop();
        }

        public void Kill()
        {
            Model.Kill();
            _movement.Stop();
        }

        public CollisionDecision HandleCollisionWith(ElfController other)
        {
            if (other == null || Model.IsDead || other.Model.IsDead)
                return CollisionDecision.None();

            return _collisionStrategy.Decide(Model, other.Model);
        }

        private void UpdateStun(float deltaTime)
        {
            if (!Model.IsStunned)
                return;

            _stunTimeRemaining -= deltaTime;

            if (_stunTimeRemaining <= 0f)
            {
                Model.RemoveStun();
                _stunTimeRemaining = 0f;
                _movement.Resume();
            }
        }
    }
}