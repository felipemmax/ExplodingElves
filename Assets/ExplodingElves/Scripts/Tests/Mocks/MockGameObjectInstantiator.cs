using System;
using System.Collections.Generic;
using ExplodingElves.Interfaces;
using UnityEngine;

namespace ExplodingElves.Tests.Mocks
{
    public class MockGameObjectInstantiator : IGameObjectInstantiator
    {
        private readonly Dictionary<GameObject, Func<GameObject>> _results = new();
        public int InstantiateCallCount { get; private set; }

        public GameObject Instantiate(GameObject prefab)
        {
            InstantiateCallCount++;
            if (prefab != null && _results.TryGetValue(prefab, out Func<GameObject> factory)) return factory();
            return new GameObject("DefaultMockInstance");
        }

        public void SetResult(GameObject prefab, Func<GameObject> factory)
        {
            _results[prefab] = factory;
        }

        public void SetResult(GameObject prefab, GameObject result)
        {
            _results[prefab] = () => result;
        }
    }
}