using System.Collections.Generic;
using ExplodingElves.Interfaces;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Core.Pooling
{
    public class PrefabPool : IPrefabPool
    {
        private const int MAX_POOL_SIZE = 100;
        private readonly Dictionary<GameObject, GameObject> _instanceToPrefab = new();
        private readonly IGameObjectInstantiator _instantiator;
        private readonly Dictionary<GameObject, Stack<GameObject>> _pools = new();
        private Transform _root;

        [Inject]
        public PrefabPool(IGameObjectInstantiator instantiator)
        {
            _instantiator = instantiator;
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

            if (!_pools.TryGetValue(prefab, out Stack<GameObject> stack))
            {
                stack = new Stack<GameObject>();
                _pools[prefab] = stack;
            }

            GameObject instance;
            if (stack.Count > 0)
            {
                instance = stack.Pop();
            }
            else if (GetTotalPoolSize() < MAX_POOL_SIZE)
            {
                instance = _instantiator.Instantiate(prefab);
            }
            else
            {
                Debug.LogWarning($"Pool size limit ({MAX_POOL_SIZE}) reached. Cannot spawn more instances.");
                return null;
            }

            _instanceToPrefab[instance] = prefab;

            Transform t = instance.transform;
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

            if (!_instanceToPrefab.TryGetValue(instance, out GameObject prefab) || prefab == null)
            {
                instance.SetActive(false);
                instance.transform.SetParent(_root, false);
                return;
            }

            if (!_pools.TryGetValue(prefab, out Stack<GameObject> stack))
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

            if (!_pools.TryGetValue(prefab, out Stack<GameObject> stack))
            {
                stack = new Stack<GameObject>();
                _pools[prefab] = stack;
            }

            int remainingSlots = MAX_POOL_SIZE - GetTotalPoolSize();
            int actualCount = Mathf.Min(count, remainingSlots);

            for (int i = 0; i < actualCount; i++)
            {
                GameObject go = _instantiator.Instantiate(prefab);
                go.SetActive(false);
                Transform t = go.transform;
                t.SetParent(parent != null ? parent : _root, false);
                stack.Push(go);
                _instanceToPrefab[go] = prefab;
            }
        }

        private int GetTotalPoolSize()
        {
            int total = 0;
            foreach (Stack<GameObject> pool in _pools.Values) total += pool.Count;
            return total;
        }

        private void EnsureRoot()
        {
            if (_root != null) return;
            GameObject existing = GameObject.Find("__PoolsRoot");
            if (existing != null)
            {
                _root = existing.transform;
                return;
            }

            GameObject rootGo = new("__PoolsRoot");
            _root = rootGo.transform;
        }
    }
}