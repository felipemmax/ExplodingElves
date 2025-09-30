namespace ExplodingElves.Core
{
    public struct CollisionDecision
    {
        public bool ShouldSpawnExtra;
        public bool ShouldExplodeBoth;

        public static CollisionDecision SpawnExtra()
        {
            return new CollisionDecision
            {
                ShouldSpawnExtra = true,
                ShouldExplodeBoth = false
            };
        }

        public static CollisionDecision ExplodeBoth()
        {
            return new CollisionDecision
            {
                ShouldSpawnExtra = false,
                ShouldExplodeBoth = true
            };
        }

        public static CollisionDecision None()
        {
            return new CollisionDecision
            {
                ShouldSpawnExtra = false,
                ShouldExplodeBoth = false
            };
        }
    }
}