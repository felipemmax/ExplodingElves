namespace ExplodingElves.Core.Characters.States
{
    public class NormalState : ElfBaseState
    {
        public NormalState(ElfStateManager stateManager) : base(stateManager)
        {
        }

        public override void Enter()
        {
            StateManager.Movement.Resume();
        }

        public override void Update(float deltaTime)
        {
            StateManager.Movement.Update(deltaTime);
        }

        public override void ApplyStun()
        {
            StateManager.TransitionToState(new StunnedState(StateManager));
        }
    }
}