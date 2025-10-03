using System;

namespace ExplodingElves.Core.Characters
{
    public class ElfStateManager
    {
        private const float StunDuration = 0.5f;

        private float _stunTimeRemaining;

        public ElfStateManager(Elf elf)
        {
            CurrentState = elf ?? throw new ArgumentNullException(nameof(elf));
        }

        public Elf CurrentState { get; }

        public bool IsDead => CurrentState.IsDead;
        public bool IsStunned => CurrentState.IsStunned;

        public void Update(float deltaTime)
        {
            if (CurrentState.IsDead || !CurrentState.IsStunned)
                return;

            _stunTimeRemaining -= deltaTime;

            if (_stunTimeRemaining <= 0f)
            {
                CurrentState.RemoveStun();
                _stunTimeRemaining = 0f;
            }
        }

        public void ApplyStun()
        {
            if (CurrentState.IsDead)
                return;

            CurrentState.ApplyStun();
            _stunTimeRemaining = StunDuration;
        }

        public void Kill()
        {
            CurrentState.MarkAsDead();
        }
    }
}