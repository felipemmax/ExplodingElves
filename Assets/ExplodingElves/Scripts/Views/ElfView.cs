using DG.Tweening;
using ExplodingElves.Core;
using ExplodingElves.Interfaces;
using ExplodingElves.Managers;
using ExplodingElves.Pools;
using ExplodingElves.Services;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Views
{
    [RequireComponent(typeof(Rigidbody))]
    public class ElfView : MonoBehaviour
    {
        private const float TweenDuration = 0.5f;
        private const float RotationSpeed = 10f;
        private const float MinSpeedForWalking = 0.1f;
        private const float DeathDespawnDelay = 6f; // Delay before despawning after death
        private const float StunDuration = 0.5f; // Duration to freeze after being hit

        [Header("Config")] [SerializeField] private Renderer targetRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private Collider elfCollider;

        private Vector3 _currentDirection;
        private ElfData _elfData;

        private Rigidbody _rigidbody;
        private bool _isDead;
        private bool _isStunned;
        private float _stunTimeRemaining;

        // Animation parameter hashes for performance
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int DeathHash = Animator.StringToHash("Death");
        private static readonly int SpawnHash = Animator.StringToHash("Spawn");
        private static readonly int HitHash = Animator.StringToHash("Hit");

        // Injected dependencies
        [Inject] public CollisionHandler CollisionHandler { get; set; }
        [Inject] public IPrefabPool Pool { get; set; }
        [Inject] public ExplosionService ExplosionService { get; set; }

        public ElfLogic Logic { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (targetRenderer == null)
                targetRenderer = GetComponentInChildren<Renderer>();
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
            if (elfCollider == null)
                elfCollider = GetComponent<Collider>();
        }

        private void Update()
        {
            if (Logic == null || _isDead)
                return;

            // Handle stun timer
            if (_isStunned)
            {
                _stunTimeRemaining -= Time.deltaTime;
                if (_stunTimeRemaining <= 0f)
                {
                    _isStunned = false;
                    _stunTimeRemaining = 0f;
                }
                return; // Skip normal logic while stunned
            }

            Logic.Tick(Time.deltaTime);

            if (Logic.ShouldChangeDirection)
            {
                PickNewDirection();
                Logic.ResetDirectionTimer();
            }

            // Update rotation to face movement direction
            UpdateRotation();
            
            // Update animation based on movement speed
            UpdateAnimation();

            if (transform.position.y < -10f)
            {
                DespawnElf();
            }
        }

        private void FixedUpdate()
        {
            if (Logic == null || _rigidbody == null || _isDead || _isStunned) return;
            ApplyMovement(_currentDirection, Logic.Speed);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_isDead) return;

            ElfView otherView = other.gameObject.GetComponent<ElfView>();

            if (otherView == null || otherView == this) return;

            // Avoid double-processing: only lower instance ID processes collision
            if (GetInstanceID() > otherView.GetInstanceID()) return;

            // Stop both elves, stun them, and change their directions
            StopAndStun();
            otherView.StopAndStun();

            // Play hit animation for both elves
            PlayHitAnimation();
            otherView.PlayHitAnimation();

            CollisionDecision decision = Logic.HandleCollision(otherView.Logic);
            Vector3 contactPoint = other.contacts.Length > 0 ? other.contacts[0].point : transform.position;

            if (decision.ShouldSpawnExtra)
            {
                Vector3 avgPos = (transform.position + otherView.transform.position) * 0.5f;
            }

            if (decision.ShouldExplodeBoth)
            {
                Explode(contactPoint);
                otherView.Explode(contactPoint);
            }
        }

        /// <summary>
        ///     Initializes the elf with data. Called by factory/pool after spawn.
        /// </summary>
        public void Initialize(ElfData data, float moveSpeed)
        {
            _elfData = data;
            _isDead = false;

            // Re-enable collider when spawning
            if (elfCollider != null)
                elfCollider.enabled = true;

            if (_rigidbody != null)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.isKinematic = false;
            }

            Logic = new ElfLogic(_elfData, CollisionHandler);
            ApplyMaterial();
            PickNewDirection();
            
            // Play spawn animation
            PlaySpawnAnimation();
        }

        private void PickNewDirection()
        {
            Vector3 dir = new(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            if (dir.sqrMagnitude < 0.001f) dir = Vector3.forward;
            _currentDirection = dir.normalized;
        }
        
        private void StopAndChangeDirection()
        {
            // Stop the elf immediately
            if (_rigidbody != null)
            {
                DOTween.Kill(_rigidbody);
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }

            // Pick a new random direction
            PickNewDirection();

            // Reset the direction change timer
            if (Logic != null)
            {
                Logic.ResetDirectionTimer();
            }
        }

        private void StopAndStun()
        {
            // Stop the elf immediately
            if (_rigidbody != null)
            {
                DOTween.Kill(_rigidbody);
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }

            // Apply stun
            _isStunned = true;
            _stunTimeRemaining = StunDuration;

            // Pick a new random direction for when stun ends
            PickNewDirection();

            // Reset the direction change timer
            if (Logic != null)
            {
                Logic.ResetDirectionTimer();
            }
        }

        private void ApplyMovement(Vector3 direction, float speed)
        {
            if (_rigidbody == null) return;

            Vector3 desiredVelocity = direction.normalized * Mathf.Max(0f, speed);
            DOTween.Kill(_rigidbody);
            DOTween.To(() => _rigidbody.linearVelocity, v => _rigidbody.linearVelocity = v, desiredVelocity,
                    TweenDuration)
                .SetTarget(_rigidbody)
                .SetEase(Ease.OutSine);
        }

        private void UpdateRotation()
        {
            if (_currentDirection.sqrMagnitude < 0.001f) return;

            // Calculate target rotation based on movement direction
            Quaternion targetRotation = Quaternion.LookRotation(_currentDirection, Vector3.up);
            
            // Smoothly rotate towards the target direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
        }

        private void UpdateAnimation()
        {
            if (animator == null) return;

            // Check if character is moving
            float currentSpeed = _rigidbody != null ? _rigidbody.linearVelocity.magnitude : 0f;
            bool isWalking = currentSpeed > MinSpeedForWalking;

            if (isWalking)
            {
                PlayWalkAnimation();
            }
            else
            {
                PlayIdleAnimation();
            }
        }

        private void PlaySpawnAnimation()
        {
            if (animator == null) return;
            animator.SetTrigger(SpawnHash);
        }

        private void PlayWalkAnimation()
        {
            if (animator == null) return;
            animator.SetBool(WalkHash, true);
            animator.SetBool(IdleHash, false);
        }

        private void PlayIdleAnimation()
        {
            if (animator == null) return;
            animator.SetBool(WalkHash, false);
            animator.SetBool(IdleHash, true);
        }

        private void PlayDeathAnimation()
        {
            if (animator == null) return;
            animator.SetTrigger(DeathHash);
        }

        private void PlayHitAnimation()
        {
            if (animator == null)
                return;
            
            animator.SetTrigger(HitHash);
        }

        private void ApplyMaterial()
        {
            if (targetRenderer != null && _elfData != null)
                targetRenderer.sharedMaterial = _elfData.ElfMaterial;
        }

        private void Explode(Vector3 position)
        {
            _isDead = true;

            // Disable collisions immediately for dead elves
            DisableCollisions();

            // Stop movement
            if (_rigidbody != null)
            {
                DOTween.Kill(_rigidbody);
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.isKinematic = true;
            }

            // Play death animation before exploding
            PlayDeathAnimation();

            ExplosionService?.PlayExplosion(position);

            // Delay despawn to allow death animation to play
            DOTween.Sequence()
                .AppendInterval(DeathDespawnDelay)
                .OnComplete(() => DespawnElf())
                .SetTarget(this);
        }

        private void DisableCollisions()
        {
            // Disable the collider to prevent any further collisions
            if (elfCollider != null)
                elfCollider.enabled = false;
        }

        private void DespawnElf()
        {
            if (Pool != null)
                Pool.Despawn(gameObject);
            else
                gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            // Clean up any active tweens when destroyed
            DOTween.Kill(this);
            DOTween.Kill(_rigidbody);
        }

        public void OnSpawned(Vector3 position, ElfData data)
        {
            throw new System.NotImplementedException();
        }

        public void OnDespawned()
        {
            throw new System.NotImplementedException();
        }
    }
}