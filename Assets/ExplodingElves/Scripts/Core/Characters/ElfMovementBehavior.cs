using System;
using ExplodingElves.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace ExplodingElves.Core.Characters
{
    public class ElfMovementBehavior
    {
        private const float WanderRadius = 20f;
        private const int MaxRandomAttempts = 10;

        private readonly INavMeshAgentWrapper _agent;
        private readonly Vector2 _directionChangeInterval;
        private float _timeSinceLastDirectionChange;

        public ElfMovementBehavior(INavMeshAgentWrapper agent, Vector2 directionChangeInterval)
        {
            _agent = agent ?? throw new ArgumentNullException(nameof(agent));
            _directionChangeInterval = directionChangeInterval;
            _timeSinceLastDirectionChange = 0f;
        }

        public void Update(float deltaTime)
        {
            _timeSinceLastDirectionChange += deltaTime;

            if (ShouldChangeDirection())
            {
                PickNewDestination();
                ResetTimer();
            }
        }

        public void Stop()
        {
            if (_agent != null && _agent.enabled)
            {
                _agent.isStopped = true;
                _agent.ResetPath();
            }
        }

        public void Resume()
        {
            if (_agent != null && _agent.enabled)
            {
                _agent.isStopped = false;
                PickNewDestination();
            }
        }

        public float GetCurrentSpeed()
        {
            if (_agent == null || !_agent.enabled)
                return 0f;

            return _agent.velocity.magnitude;
        }

        private bool ShouldChangeDirection()
        {
            float changeTime = Random.Range(_directionChangeInterval.x, _directionChangeInterval.y);
            return _timeSinceLastDirectionChange >= changeTime;
        }

        private void PickNewDestination()
        {
            if (_agent == null || !_agent.enabled)
                return;

            if (TryGetRandomNavMeshPoint(_agent.transform.position, WanderRadius, out Vector3 destination))
                _agent.SetDestination(destination);
        }

        private void ResetTimer()
        {
            _timeSinceLastDirectionChange = 0f;
        }

        private bool TryGetRandomNavMeshPoint(Vector3 center, float radius, out Vector3 result)
        {
            for (int i = 0; i < MaxRandomAttempts; i++)
            {
                Vector2 randomPoint = Random.insideUnitCircle * radius;
                Vector3 candidate = center + new Vector3(randomPoint.x, 0, randomPoint.y);

                if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, radius, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }

            result = center;
            return false;
        }
    }
}