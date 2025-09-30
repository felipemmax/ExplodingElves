using System.Collections.Generic;
using ExplodingElves.Core;
using UnityEngine;

namespace ExplodingElves.Services
{
    public class SpawnCooldownService
    {
        private readonly Dictionary<ElfColor, float> _lastTimeByColor = new();
        private float _cooldownSeconds;

        public SpawnCooldownService(float cooldownSeconds)
        {
            _cooldownSeconds = Mathf.Max(0f, cooldownSeconds);
        }

        public float CooldownSeconds
        {
            get => _cooldownSeconds;
            set => _cooldownSeconds = Mathf.Max(0f, value);
        }

        public bool TryRequest(ElfColor color)
        {
            float now = Time.time;
            if (_lastTimeByColor.TryGetValue(color, out float last))
                if (now - last < _cooldownSeconds)
                    return false;

            _lastTimeByColor[color] = now;
            return true;
        }
    }
}