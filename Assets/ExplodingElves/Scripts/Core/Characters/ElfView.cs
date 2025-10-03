using ExplodingElves.Interfaces;
using ExplodingElves.Services;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace ExplodingElves.Core.Characters
{
    /// <summary>
    ///     View: Handles only Unity-specific rendering, animation, and physics callbacks.
    ///     Single Responsibility: Visual representation of the elf.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Collider))]
    public class ElfView : MonoBehaviour
    {
        private const float DeathDespawnDelay = 6f;

        // Animation hashes
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int DeathHash = Animator.StringToHash("Death");
        private static readonly int SpawnHash = Animator.StringToHash("Spawn");
        private static readonly int HitHash = Animator.StringToHash("Hit");

        [Header("Visual Components")] [SerializeField]
        private Renderer targetRenderer;

        [SerializeField] private Animator animator;
        [SerializeField] private Collider elfCollider;

        private NavMeshAgent _agent;

        [Inject] public IElfCollisionStrategy CollisionStrategy { get; set; }
        [Inject] public IPrefabPool Pool { get; set; }
        [Inject] public VFXService VFXService { get; set; }
        [Inject] public ElfSpawnCooldownService CooldownService { get; set; }
        [Inject] public SpawnerRegistryService SpawnerRegistry { get; set; }

        public ElfController Controller { get; private set; }

        private void Awake()
        {
            SetupComponents();
        }

        private void Update()
        {
            if (Controller == null)
                return;

            Controller.Update(Time.deltaTime);
            UpdateAnimation();
            CheckFallDeath();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Controller == null || Controller.Model.IsDead)
                return;

            ElfView otherView = other.GetComponent<ElfView>();
            if (otherView == null || otherView == this)
                return;

            // Prevent double processing
            if (GetInstanceID() > otherView.GetInstanceID())
                return;

            HandleCollision(otherView);
        }

        public void Initialize(ElfData data, Vector3 spawnPosition)
        {
            Elf elf = new(data);
            Controller = new ElfController(elf, _agent, CollisionStrategy);

            WarpToPosition(spawnPosition);
            ApplyVisuals(data);
            EnableCollisions();
            PlayAnimation(SpawnHash);
            
            // Play spawn VFX
            VFXService?.PlaySpawnVFX(spawnPosition);
        }

        public void Despawn()
        {
            if (Pool != null)
                Pool.Despawn(gameObject);
            else
                gameObject.SetActive(false);
        }

        private void SetupComponents()
        {
            _agent = GetComponent<NavMeshAgent>();

            if (targetRenderer == null)
                targetRenderer = GetComponentInChildren<Renderer>();
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
            if (elfCollider == null)
                elfCollider = GetComponent<Collider>();

            if (elfCollider != null)
            {
                elfCollider.isTrigger = true;
                DisableCollisions();
                Invoke(nameof(EnableCollisions), 3);
            }
        }

        private void WarpToPosition(Vector3 position)
        {
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                _agent.Warp(hit.position);
            }
            else
            {
                Debug.LogWarning($"{name} spawned outside NavMesh at {position}");
                transform.position = position;
            }
        }

        private void ApplyVisuals(ElfData data)
        {
            if (targetRenderer != null && data.ElfMaterial != null)
                targetRenderer.sharedMaterial = data.ElfMaterial;

            _agent.speed = data.Speed;
        }

        private void UpdateAnimation()
        {
            if (animator == null || Controller == null)
                return;

            bool isWalking = Controller.IsMoving && !Controller.Model.IsStunned && !Controller.Model.IsDead;

            animator.SetBool(WalkHash, isWalking);
            animator.SetBool(IdleHash, !isWalking);
        }

        private void CheckFallDeath()
        {
            if (transform.position.y < -10f) Despawn();
        }

        private void PlayAnimation(int animationHash)
        {
            if (animator != null)
                animator.SetTrigger(animationHash);
        }

        private void EnableCollisions()
        {
            elfCollider.enabled = true;
        }

        private void DisableCollisions()
        {
            elfCollider.enabled = false;
        }

        private void HandleCollision(ElfView otherView)
        {
            // Apply stun to both
            Controller.ApplyStun();
            otherView.Controller.ApplyStun();

            // Play hit animations
            PlayAnimation(HitHash);
            otherView.PlayAnimation(HitHash);

            // Get collision decision
            CollisionDecision decision = Controller.HandleCollisionWith(otherView.Controller);
            Vector3 collisionPoint = transform.position;

            // Execute decision
            if (decision.ShouldSpawnExtra)
            {
                // Play collision VFX for same-color collision
                VFXService?.PlayCollisionVFX(collisionPoint);
                RequestSpawnFromSpawner(otherView);
            }

            if (decision.ShouldExplodeBoth)
            {
                Explode(collisionPoint);
                otherView.Explode(collisionPoint);
            }
        }

        private void RequestSpawnFromSpawner(ElfView otherView)
        {
            if (SpawnerRegistry == null || CooldownService == null)
            {
                Debug.LogWarning("Required services not injected");
                return;
            }

            // Check GLOBAL cooldown
            if (!CooldownService.CanSpawn())
            {
                float remaining = CooldownService.GetRemainingCooldown();
                Debug.Log($"Spawn on cooldown ({remaining:F2}s remaining)");
                return;
            }

            // Get the color of the colliding elves (they're the same color)
            ElfColor colorToSpawn = Controller.Model.Color;

            // Request spawn from the appropriate spawner
            bool spawned = SpawnerRegistry.RequestSpawn(colorToSpawn);

            if (spawned)
            {
                // Register the spawn to start cooldown
                CooldownService.RegisterSpawn();
                Debug.Log($"Extra {colorToSpawn} elf spawn requested from spawner");
            }
        }

        private void Explode(Vector3 position)
        {
            Controller.Kill();
            DisableCollisions();
            PlayAnimation(DeathHash);

            VFXService?.PlayExplosion(position);

            Invoke(nameof(Despawn), DeathDespawnDelay);
        }

        // Pool lifecycle hooks
        public void OnSpawned(Vector3 position, ElfData data)
        {
            Initialize(data, position);
        }

        public void OnDespawned()
        {
            Controller = null;

            if (_agent != null)
            {
                _agent.isStopped = true;
                _agent.ResetPath();
            }
        }
    }
}