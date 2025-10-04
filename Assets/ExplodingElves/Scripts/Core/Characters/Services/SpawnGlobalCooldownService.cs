using ExplodingElves.Interfaces;
using UnityEngine;

namespace ExplodingElves.Core.Characters.Services
{
    public class SpawnGlobalCooldownService : ISpawnCooldownService
    {
        private readonly float _cooldownDuration;
        private float _lastSpawnTime = -999f;

        public SpawnGlobalCooldownService(float cooldownDuration)
        {
            _cooldownDuration = Mathf.Max(0f, cooldownDuration);
        }

        public bool CanSpawn()
        {
            return Time.time >= _lastSpawnTime + _cooldownDuration;
        }

        public void RegisterSpawn()
        {
            _lastSpawnTime = Time.time;
        }

        public float GetRemainingCooldown()
        {
            float remaining = _lastSpawnTime + _cooldownDuration - Time.time;
            return Mathf.Max(0f, remaining);
        }
    }
}