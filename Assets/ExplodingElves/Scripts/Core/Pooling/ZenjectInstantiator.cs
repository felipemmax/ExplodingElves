using ExplodingElves.Interfaces;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Core.Pooling
{
    public class ZenjectInstantiator : IGameObjectInstantiator
    {
        private readonly DiContainer _container;

        public ZenjectInstantiator(DiContainer container)
        {
            _container = container;
        }

        public GameObject Instantiate(GameObject prefab)
        {
            return _container.InstantiatePrefab(prefab);
        }
    }
}