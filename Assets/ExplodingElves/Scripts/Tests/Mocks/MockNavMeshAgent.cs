using UnityEngine;
using UnityEngine.AI;

namespace ExplodingElves.Tests.Mocks
{
    public class MockNavMeshAgent : MonoBehaviour
    {
        public bool isStopped { get; set; }
        public Vector3 velocity { get; set; } = Vector3.zero;
        public bool enabled { get; set; } = true;
        public float speed { get; set; } = 3f;

        public int ResetPathCallCount { get; private set; }
        public int SetDestinationCallCount { get; private set; }
        public int WarpCallCount { get; private set; }
        public Vector3 LastDestination { get; private set; }
        public Vector3 LastWarpPosition { get; private set; }

        public void ResetPath()
        {
            ResetPathCallCount++;
        }

        public bool SetDestination(Vector3 target)
        {
            SetDestinationCallCount++;
            LastDestination = target;
            return true;
        }

        public bool Warp(Vector3 newPosition)
        {
            WarpCallCount++;
            LastWarpPosition = newPosition;
            transform.position = newPosition;
            return true;
        }

        // Implicit conversion from NavMeshAgent for easier testing
        public static implicit operator NavMeshAgent(MockNavMeshAgent mock)
        {
            // This won't work, but we'll handle it differently
            return null;
        }
    }
}