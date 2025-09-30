using UnityEngine;

namespace ExplodingElves.Interfaces
{
    /// <summary>
    ///     Generic prefab-based pool service that can pool any kind of GameObject prefab.
    ///     Provides spawn and despawn operations and optional warmup.
    /// </summary>
    public interface IPrefabPool
    {
        /// <summary>
        ///     Spawn an instance of a prefab from the pool (or instantiate if none available).
        /// </summary>
        /// <param name="prefab">The prefab to spawn.</param>
        /// <param name="position">World position.</param>
        /// <param name="rotation">World rotation.</param>
        /// <param name="parent">Optional parent transform. If null, a hidden pool root will be used.</param>
        /// <returns>The spawned GameObject instance.</returns>
        GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null);

        /// <summary>
        ///     Return an instance back to its corresponding prefab pool.
        /// </summary>
        /// <param name="instance">Spawned instance to return.</param>
        void Despawn(GameObject instance);

        /// <summary>
        ///     Pre-create a number of instances for a given prefab and keep them disabled in the pool.
        /// </summary>
        /// <param name="prefab">The prefab to warm up.</param>
        /// <param name="count">Number of instances to pre-create.</param>
        /// <param name="parent">Optional parent for organizing spawned objects.</param>
        void Warmup(GameObject prefab, int count, Transform parent = null);
    }
}