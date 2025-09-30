using System.Collections.Generic;
using ExplodingElves.Interfaces;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Pools
{
    /// <summary>
    /// Generic prefab-based pool that can be used for any GameObject assets (FX, UI elements, projectiles, etc.).
    /// Internally uses Zenject's DiContainer for instantiation to preserve DI on pooled prefabs.
    /// </summary>
    public class PrefabPool : IPrefabPool
    {
        private readonly DiContainer _container;
        private readonly Dictionary<GameObject, Stack<GameObject>> _pools = new();
        private readonly Dictionary<GameObject, GameObject> _instanceToPrefab = new();
        private Transform _root;

        [Inject]
        public PrefabPool(DiContainer container)
        {
            _container = container;
            EnsureRoot();
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null)
            {
                Debug.LogWarning("PrefabPool.Spawn called with null prefab");
                return null;
            }

            EnsureRoot();

            if (!_pools.TryGetValue(prefab, out var stack))
            {
                stack = new Stack<GameObject>();
                _pools[prefab] = stack;
            }

            GameObject instance;
            if (stack.Count > 0)
            {
                instance = stack.Pop();
            }
            else
            {
                instance = _container.InstantiatePrefab(prefab);
            }

            _instanceToPrefab[instance] = prefab;

            var t = instance.transform;
            if (parent == null)
                t.SetParent(_root, false);
            else
                t.SetParent(parent, false);

            t.position = position;
            t.rotation = rotation;
            instance.SetActive(true);
            return instance;
        }

        public void Despawn(GameObject instance)
        {
            if (instance == null) return;

            if (!_instanceToPrefab.TryGetValue(instance, out var prefab) || prefab == null)
            {
                // If instance was not created by this pool, just disable it to avoid Destroy churn.
                instance.SetActive(false);
                instance.transform.SetParent(_root, false);
                return;
            }

            if (!_pools.TryGetValue(prefab, out var stack))
            {
                stack = new Stack<GameObject>();
                _pools[prefab] = stack;
            }

            instance.SetActive(false);
            instance.transform.SetParent(_root, false);
            stack.Push(instance);
        }

        public void Warmup(GameObject prefab, int count, Transform parent = null)
        {
            if (prefab == null || count <= 0) return;
            EnsureRoot();

            if (!_pools.TryGetValue(prefab, out var stack))
            {
                stack = new Stack<GameObject>();
                _pools[prefab] = stack;
            }

            for (int i = 0; i < count; i++)
            {
                var go = _container.InstantiatePrefab(prefab);
                go.SetActive(false);
                var t = go.transform;
                t.SetParent(parent != null ? parent : _root, false);
                stack.Push(go);
                _instanceToPrefab[go] = prefab;
            }
        }

        private void EnsureRoot()
        {
            if (_root != null) return;
            var existing = GameObject.Find("__PoolsRoot");
            if (existing != null)
            {
                _root = existing.transform;
                return;
            }

            var rootGo = new GameObject("__PoolsRoot");
            Object.DontDestroyOnLoad(rootGo);
            _root = rootGo.transform;
        }
    }
}
