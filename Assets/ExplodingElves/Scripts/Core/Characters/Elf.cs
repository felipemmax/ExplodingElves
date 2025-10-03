using System;
using UnityEngine;

namespace ExplodingElves.Core.Characters
{
    public class Elf
    {
        public Elf(ElfData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            IsDead = false;
            IsStunned = false;
        }

        public ElfData Data { get; }
        public ElfColor Color => Data.ElfColor;
        public float Speed => Data.Speed;
        public bool IsDead { get; set; }
        public bool IsStunned { get; set; }
        public Vector3 Position { get; set; }

        public void Kill()
        {
            IsDead = true;
        }

        public void Stun()
        {
            IsStunned = true;
        }

        public void RemoveStun()
        {
            IsStunned = false;
        }
    }
}