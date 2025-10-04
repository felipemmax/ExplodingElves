using ExplodingElves.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace ExplodingElves.Core.Characters
{
    public class NavMeshAgentWrapper : INavMeshAgentWrapper
    {
        private readonly NavMeshAgent _agent;

        public NavMeshAgentWrapper(NavMeshAgent agent)
        {
            _agent = agent;
        }

        public bool isStopped
        {
            get => _agent.isStopped;
            set => _agent.isStopped = value;
        }

        public Vector3 velocity => _agent.velocity;

        public bool enabled
        {
            get => _agent.enabled;
            set => _agent.enabled = value;
        }

        public Transform transform => _agent.transform;

        public void ResetPath()
        {
            _agent.ResetPath();
        }

        public bool SetDestination(Vector3 target)
        {
            return _agent.SetDestination(target);
        }

        public bool Warp(Vector3 newPosition)
        {
            return _agent.Warp(newPosition);
        }
    }
}