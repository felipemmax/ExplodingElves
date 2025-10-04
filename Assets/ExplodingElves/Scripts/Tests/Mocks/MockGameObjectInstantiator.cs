using System;
using System.Collections.Generic;
using ExplodingElves.Interfaces;
using UnityEngine;

namespace ExplodingElves.Tests.Mocks
{
    public class MockGameObjectInstantiator : IGameObjectInstantiator
    {
        public int InstantiateCallCount { get; private set; }
        private readonly Dictionary<GameObject, Func<GameObject>> _results = new();

        public GameObject Instantiate(GameObject prefab)
        {
            InstantiateCallCount++;
            if (prefab != null && _results.TryGetValue(prefab, out var factory))
            {
                return factory();
            }
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