using ExplodingElves.Core.Collision;
using ExplodingElves.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace ExplodingElves.Core.Characters
{
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

        // Dependencies injected via Zenject
        [Inject] public IElfCollisionStrategy CollisionStrategy { get; set; }
        [Inject] public IPrefabPool Pool { get; set; }
        [Inject] public IVFXService VFXService { get; set; }
        [Inject] public ElfCollisionHandler CollisionHandler { get; set; }

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
            if (Controller == null || Controller.IsDead)
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

            bool isWalking = Controller.IsMoving && !Controller.IsStunned && !Controller.IsDead;

            animator.SetBool(WalkHash, isWalking);
            animator.SetBool(IdleHash, !isWalking);
        }

        private void CheckFallDeath()
        {
            if (transform.position.y < -10f)
                Despawn();
        }

        private void PlayAnimation(int animationHash)
        {
            animator?.SetTrigger(animationHash);
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
            Vector3 collisionPoint = transform.position;

            // Play hit animations immediately
            PlayAnimation(HitHash);
            otherView.PlayAnimation(HitHash);

            // Delegate collision logic to handler
            ElfCollisionHandler.CollisionResult result =
                CollisionHandler.ProcessCollision(Controller, otherView.Controller);

            // Execute visual results
            if (result.ShouldSpawnExtra)
                VFXService?.PlayCollisionVFX(collisionPoint);

            if (result.ShouldExplodeBoth)
            {
                Explode(collisionPoint);
                otherView.Explode(collisionPoint);
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

        public void OnSpawned(Vector3 position, ElfData data)
        {
            Initialize(data, position);
        }
    }
}