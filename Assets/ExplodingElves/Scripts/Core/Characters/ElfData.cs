using UnityEngine;

namespace ExplodingElves.Core.Characters
{
    [CreateAssetMenu(fileName = "ElfData", menuName = "ExplodingElves/Elf Data")]
    public class ElfData : ScriptableObject
    {
        [Header("Movement")] [SerializeField] [Min(0f)]
        private float speed = 3f;

        [SerializeField] [Min(0.1f)] private float movementSmoothTime = 0.5f;
        [SerializeField] private Vector2 directionChangeInterval = new(2f, 5f);

        [Header("Appearance")] [SerializeField]
        private ElfColor elfColor;

        [SerializeField] private Material elfMaterial;

        public float Speed => speed;
        public float MovementSmoothTime => movementSmoothTime;
        public Vector2 DirectionChangeInterval => directionChangeInterval;
        public ElfColor ElfColor => elfColor;
        public Material ElfMaterial => elfMaterial;

        private void OnValidate()
        {
            // Ensure interval min is never greater than max
            if (directionChangeInterval.x > directionChangeInterval.y)
                directionChangeInterval.y = directionChangeInterval.x;
        }
    }
}