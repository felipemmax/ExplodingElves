using ExplodingElves.Interfaces;
using UnityEngine;

namespace ExplodingElves.Tests.Mocks
{
    public class MockNavMeshAgentWrapper : INavMeshAgentWrapper
    {
        private readonly GameObject _gameObject;

        public MockNavMeshAgentWrapper()
        {
            _gameObject = new GameObject("MockNavMeshAgent");
            transform = _gameObject.transform;
        }

        public int ResetPathCallCount { get; private set; }
        public int SetDestinationCallCount { get; private set; }
        public int WarpCallCount { get; private set; }
        public Vector3 LastDestination { get; private set; }
        public Vector3 LastWarpPosition { get; private set; }

        public bool isStopped { get; set; }
        public Vector3 velocity { get; set; } = Vector3.zero;
        public bool enabled { get; set; } = true;
        public Transform transform { get; }

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

        public void Destroy()
        {
            if (_gameObject != null)
                Object.DestroyImmediate(_gameObject);
        }
    }
}