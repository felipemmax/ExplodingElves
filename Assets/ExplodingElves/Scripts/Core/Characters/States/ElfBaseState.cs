namespace ExplodingElves.Core.Characters.States
{
    public abstract class ElfBaseState : IElfState
    {
        protected readonly ElfStateManager StateManager;

        protected ElfBaseState(ElfStateManager stateManager)
        {
            StateManager = stateManager;
        }

        public virtual void Enter()
        {
        }

        public virtual void Update(float deltaTime)
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void ApplyStun()
        {
        }

        public virtual void Kill()
        {
            StateManager.TransitionToState(new DeadState(StateManager));
        }
    }
}