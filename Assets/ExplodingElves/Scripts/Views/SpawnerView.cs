using System.Collections;
using ExplodingElves.Core;
using ExplodingElves.Interfaces;
using ExplodingElves.Pools;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Views
{
    public class SpawnerView : MonoBehaviour, ISpawner
    {
        private const float MinInterval = 0.1f;
        public const float DefaultInterval = 5f;

        [SerializeField] private ElfColor elfColor = ElfColor.Black;
        [SerializeField] [Min(0f)] private float interval = DefaultInterval;
        [SerializeField] [Min(0f)] private float spawnRadius = 1.5f;
        [SerializeField] private ElfData elfData;
        [SerializeField] private GameObject prefab;

        private IPrefabPool _pool;
        private Coroutine _spawnLoop;

        private void OnEnable()
        {
            StartSpawnLoop();
        }

        private void OnDisable()
        {
            StopSpawnLoop();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }

        private void OnValidate()
        {
            // Ensure we have valid data in editor
            if (elfData == null) Debug.LogWarning($"SpawnerView on {gameObject.name} has no ElfData assigned!", this);
        }

        public ElfColor ElfColor => elfColor;
        public float Interval => interval;

        public void SetInterval(float newInterval)
        {
            interval = Mathf.Max(0f, newInterval);
            if (isActiveAndEnabled)
                RestartSpawnLoop();
        }

        public void Spawn()
        {
            if (_pool == null)
            {
                Debug.LogError($"ElfPool not injected into SpawnerView on {gameObject.name}", this);
                return;
            }

            if (elfData == null)
            {
                Debug.LogError($"ElfData not assigned on SpawnerView {gameObject.name}", this);
                return;
            }

            Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPos.y = transform.position.y; // Keep on same Y level

            _pool.Spawn(prefab, spawnPos, Quaternion.identity);
        }

        [Inject]
        private void Construct(IPrefabPool pool)
        {
            _pool = pool;
        }

        private void StartSpawnLoop()
        {
            if (_spawnLoop == null)
                _spawnLoop = StartCoroutine(SpawnRoutine());
        }

        private void StopSpawnLoop()
        {
            if (_spawnLoop != null)
            {
                StopCoroutine(_spawnLoop);
                _spawnLoop = null;
            }
        }

        private void RestartSpawnLoop()
        {
            StopSpawnLoop();
            StartSpawnLoop();
        }

        private IEnumerator SpawnRoutine()
        {
            while (enabled)
            {
                float waitTime = Mathf.Max(MinInterval, interval);

                Spawn();
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}