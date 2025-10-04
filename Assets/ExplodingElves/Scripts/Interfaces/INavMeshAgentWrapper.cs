using UnityEngine;

namespace ExplodingElves.Interfaces
{
    public interface INavMeshAgentWrapper
    {
        bool isStopped { get; set; }
        Vector3 velocity { get; }
        bool enabled { get; set; }
        Transform transform { get; }

        void ResetPath();
        bool SetDestination(Vector3 target);
        bool Warp(Vector3 newPosition);
    }
}