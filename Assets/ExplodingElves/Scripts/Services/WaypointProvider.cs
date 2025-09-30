using ExplodingElves.Interfaces;
using UnityEngine;

namespace ExplodingElves.Services
{
    /// <summary>
    /// Provides random points inside a configurable floor bounds. Validates by raycasting to floor layer.
    /// </summary>
    public class WaypointProvider
    {
        private readonly Vector3 _center;
        private readonly Vector3 _size;
        private readonly int _floorLayerMask;

        public WaypointProvider(Vector3 center, Vector3 size, int floorLayerMask)
        {
            _center = center;
            _size = new Vector3(Mathf.Max(0.1f, size.x), Mathf.Max(0.1f, size.y), Mathf.Max(0.1f, size.z));
            _floorLayerMask = floorLayerMask;
        }

        public Vector3 GetRandomWaypoint()
        {
            // Try a few times to find a valid point above floor
            for (int i = 0; i < 8; i++)
            {
                Vector3 p = new Vector3(
                    _center.x + (Random.value - 0.5f) * _size.x,
                    _center.y + _size.y * 0.5f + 5f, // start ray a bit above bounds
                    _center.z + (Random.value - 0.5f) * _size.z);

                if (Physics.Raycast(p, Vector3.down, out var hit, 50f, _floorLayerMask))
                {
                    return hit.point;
                }
            }

            // Fallback: flat point at center
            return _center;
        }
    }
}