using System;

namespace ExplodingElves.Core.Characters
{
    public class Elf
    {
        public Elf(ElfData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public ElfData Data { get; }
        public ElfColor Color => Data.ElfColor;
        public float Speed => Data.Speed;
    }
}