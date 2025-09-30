using ExplodingElves.Core;
using ExplodingElves.Managers;
using ExplodingElves.Pools;
using ExplodingElves.Services;
using UnityEngine;
using Zenject;
using DG.Tweening;

namespace ExplodingElves.Views
{
    /// <summary>
    /// Presentation layer for an Elf entity.
    /// Handles rendering, physics, and Unity lifecycle integration.
    /// SRP: Only manages view-layer concerns and delegates logic to ElfLogic.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ElfView : MonoBehaviour
    {
        private const float TweenDuration = 0.5f;

        [Header("Config")] 
        [SerializeField] private Renderer targetRenderer;

        // Injected dependencies
        [Inject] public CollisionHandler CollisionHandler { get; set; }
        [Inject] public SpawnManager SpawnManager { get; set; }
        [Inject] public ElfPool Pool { get; set; }
        [Inject] public ExplosionService ExplosionService { get; set; }

        private Rigidbody _rigidbody;
        private Vector3 _currentDirection;
        private ElfData _elfData;

        public ElfLogic Logic { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (targetRenderer == null) 
                targetRenderer = GetComponentInChildren<Renderer>();
        }

        private void Update()
        {
            if (Logic == null) return;
            
            Logic.Tick(Time.deltaTime);
            
            if (Logic.ShouldChangeDirection)
            {
                PickNewDirection();
                Logic.ResetDirectionTimer();
            }
        }

        private void FixedUpdate()
        {
            if (Logic == null || _rigidbody == null) return;
            ApplyMovement(_currentDirection, Logic.Speed);
        }

        private void OnCollisionEnter(Collision other)
        {
            ElfView otherView = other.gameObject.GetComponent<ElfView>();
            
            if (otherView == null || otherView == this) return;

            // Avoid double-processing: only lower instance ID processes collision
            if (GetInstanceID() > otherView.GetInstanceID()) return;

            CollisionDecision decision = Logic.HandleCollision(otherView.Logic);
            Vector3 contactPoint = other.contacts.Length > 0 ? other.contacts[0].point : transform.position;

            if (decision.ShouldSpawnExtra)
            {
                Vector3 avgPos = (transform.position + otherView.transform.position) * 0.5f;
                SpawnManager.SpawnExtra(_elfData, avgPos);
            }

            if (decision.ShouldExplodeBoth)
            {
                Explode(contactPoint);
                otherView.Explode(contactPoint);
            }
        }

        /// <summary>
        /// Initializes the elf with data. Called by factory/pool after spawn.
        /// </summary>
        public void Initialize(ElfData data, float moveSpeed)
        {
            _elfData = data;
            
            if (_rigidbody != null)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
            
            Logic = new ElfLogic(_elfData, CollisionHandler);
            ApplyMaterial();
            PickNewDirection();
        }

        private void PickNewDirection()
        {
            Vector3 dir = new(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            if (dir.sqrMagnitude < 0.001f) dir = Vector3.forward;
            _currentDirection = dir.normalized;
        }

        private void ApplyMovement(Vector3 direction, float speed)
        {
            if (_rigidbody == null) return;
            
            Vector3 desiredVelocity = direction.normalized * Mathf.Max(0f, speed);
            DOTween.Kill(_rigidbody);
            DOTween.To(() => _rigidbody.linearVelocity, v => _rigidbody.linearVelocity = v, desiredVelocity, TweenDuration)
                .SetTarget(_rigidbody)
                .SetEase(Ease.OutSine);
        }

        private void ApplyMaterial()
        {
            if (targetRenderer != null && _elfData != null)
                targetRenderer.sharedMaterial = _elfData.ElfMaterial;
        }

        private void Explode(Vector3 position)
        {
            ExplosionService?.PlayExplosion(position);
            
            if (Pool != null)
                Pool.Despawn(this);
            else
                gameObject.SetActive(false);
        }
    }
}