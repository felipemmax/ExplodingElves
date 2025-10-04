using ExplodingElves.Core.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace ExplodingElves.Core.Spawners.Views
{
    public class SpawnerView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] [Min(0f)] private float spawnRadius = 1.5f;
        [SerializeField] private ElfData elfData;
        [SerializeField] private GameObject elfPrefab;

        [SerializeField] private TextMeshPro spawnRateDisplay;

        private ISpawner _spawner;

        public ElfColor ElfColor => elfData.ElfColor;

        private void OnEnable()
        {
            _spawner.OnStateChanged += UpdateSpawnRateDisplay;
            _spawner.Start();
            UpdateSpawnRateDisplay();
        }

        private void OnDisable()
        {
            _spawner.OnStateChanged -= UpdateSpawnRateDisplay;
            _spawner.Stop();
        }

        private void OnDestroy()
        {
            _spawner.Dispose();
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
            if (elfData == null)
                Debug.LogWarning($"SpawnerView on {gameObject.name} has no ElfData assigned!", this);
            if (elfPrefab == null)
                Debug.LogWarning($"SpawnerView on {gameObject.name} has no Elf Prefab assigned!", this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _spawner.ChangeSpawnRate();
        }

        [Inject]
        private void Construct(IFactory<SpawnerData, ISpawner> spawnerFactory)
        {
            SpawnerData spawnerData = new(spawnRadius, elfData, elfPrefab, transform.position);
            _spawner = spawnerFactory.Create(spawnerData);
        }

        private void UpdateSpawnRateDisplay()
        {
            if (spawnRateDisplay != null)
                spawnRateDisplay.text = _spawner.SpawnRateDisplay;
        }
    }
}