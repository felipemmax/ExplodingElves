using ExplodingElves.Core.Characters;

namespace ExplodingElves.Interfaces
{
    /// <summary>
    ///     Abstraction of a spawner component that can spawn elves and adjust its own interval.
    ///     SRP: only spawns and manages its interval.
    /// </summary>
    public interface ISpawner
    {
        /// <summary>
        ///     The color/type this spawner is responsible for.
        /// </summary>
        ElfColor ElfColor { get; }

        /// <summary>
        ///     Current interval (seconds) between spawns.
        /// </summary>
        float Interval { get; }

        /// <summary>
        ///     Set the new interval (seconds) between spawns.
        /// </summary>
        /// <param name="interval">Interval in seconds.</param>
        void SetInterval(float interval);

        /// <summary>
        ///     Immediately spawn one elf of its color.
        /// </summary>
        void Spawn();
    }
}