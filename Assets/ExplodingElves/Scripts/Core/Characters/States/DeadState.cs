namespace ExplodingElves.Core.Characters.States
{
    public class DeadState : ElfBaseState
    {
        public DeadState(ElfStateManager stateManager) : base(stateManager)
        {
        }

        public override void Enter()
        {
            StateManager.Movement.Stop();
        }

        public override void ApplyStun()
        {
            // Cannot be stunned if dead
        }

        public override void Kill()
        {
            // Already dead
        }
    }
}