using System;

namespace ExplodingElves.Core.Characters.States
{
    public class ElfStateManager
    {
        public ElfStateManager(Elf elf, ElfMovementBehavior movement)
        {
            Elf = elf ?? throw new ArgumentNullException(nameof(elf));
            Movement = movement ?? throw new ArgumentNullException(nameof(movement));
            TransitionToState(new NormalState(this));
        }

        public IElfState CurrentState { get; private set; }
        public Elf Elf { get; }
        public ElfMovementBehavior Movement { get; }

        public bool IsDead => CurrentState is DeadState;
        public bool IsStunned => CurrentState is StunnedState;

        public void Update(float deltaTime)
        {
            CurrentState?.Update(deltaTime);
        }

        public void ApplyStun()
        {
            CurrentState?.ApplyStun();
        }

        public void Kill()
        {
            CurrentState?.Kill();
        }

        public void TransitionToState(IElfState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}