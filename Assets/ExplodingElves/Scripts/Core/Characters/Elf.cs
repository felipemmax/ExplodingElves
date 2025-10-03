using System;

namespace ExplodingElves.Core.Characters
{
    public class Elf
    {
        public Elf(ElfData data, bool isDead = false, bool isStunned = false)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            IsDead = isDead;
            IsStunned = isStunned;
        }

        public ElfData Data { get; }
        public ElfColor Color => Data.ElfColor;
        public float Speed => Data.Speed;
        public bool IsDead { get; private set; }
        public bool IsStunned { get; private set; }

        public void MarkAsDead()
        {
            IsDead = true;
        }

        public void ApplyStun()
        {
            IsStunned = true;
        }

        public void RemoveStun()
        {
            IsStunned = false;
        }
    }
}