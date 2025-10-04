using ExplodingElves.Interfaces;
using UnityEngine;

namespace ExplodingElves.Core.Services
{
    public class ElfSpawnCooldownService : ISpawnCooldownService
    {
        private readonly float _cooldownDuration;
        private float _lastSpawnTime = -999f;

        public ElfSpawnCooldownService(float cooldownDuration)
        {
            _cooldownDuration = Mathf.Max(0f, cooldownDuration);
        }

        /// <summary>
        ///     Check if enough time has passed since the last collision spawn.
        /// </summary>
        public bool CanSpawn()
        {
            return Time.time >= _lastSpawnTime + _cooldownDuration;
        }

        /// <summary>
        ///     Register that a spawn occurred, starting the cooldown.
        /// </summary>
        public void RegisterSpawn()
        {
            _lastSpawnTime = Time.time;
        }

        /// <summary>
        ///     Get remaining cooldown time in seconds.
        /// </summary>
        public float GetRemainingCooldown()
        {
            float remaining = _lastSpawnTime + _cooldownDuration - Time.time;
            return Mathf.Max(0f, remaining);
        }
    }
}