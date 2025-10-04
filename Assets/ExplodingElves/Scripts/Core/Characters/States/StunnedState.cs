namespace ExplodingElves.Core.Characters.States
{
    public class StunnedState : ElfBaseState
    {
        private const float StunDuration = 0.5f;
        private float _stunTimeRemaining;

        public StunnedState(ElfStateManager stateManager) : base(stateManager)
        {
        }

        public override void Enter()
        {
            _stunTimeRemaining = StunDuration;
            StateManager.Movement.Stop();
        }

        public override void Update(float deltaTime)
        {
            _stunTimeRemaining -= deltaTime;
            if (_stunTimeRemaining <= 0) StateManager.TransitionToState(new NormalState(StateManager));
        }

        public override void ApplyStun()
        {
            _stunTimeRemaining = StunDuration;
        }
    }
}