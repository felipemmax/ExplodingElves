using ExplodingElves.Interfaces;
using UnityEngine;

namespace ExplodingElves.Core.Pooling
{
    public class AutoReturnToPool : MonoBehaviour
    {
        private float _duration;
        private IPrefabPool _pool;
        private bool _running;
        private float _timer;

        private void Update()
        {
            if (!_running) return;
            _timer += Time.deltaTime;

            if (_timer >= _duration)
            {
                _running = false;
                _pool?.Despawn(gameObject);
            }
        }

        private void OnDisable()
        {
            // Reset timer so reused instances don't carry over time
            _timer = 0f;
            _running = false;
        }

        public void Begin(float seconds, IPrefabPool pool)
        {
            _duration = Mathf.Max(0f, seconds);
            _pool = pool;
            _timer = 0f;
            _running = true;
            enabled = true;
        }
    }
}