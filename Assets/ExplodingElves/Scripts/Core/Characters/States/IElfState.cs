namespace ExplodingElves.Core.Characters.States
{
    public interface IElfState
    {
        void Enter();
        void Update(float deltaTime);
        void Exit();
        void ApplyStun();
        void Kill();
    }
}