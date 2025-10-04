using UnityEngine;

namespace ExplodingElves.Interfaces
{
    public interface IGameObjectInstantiator
    {
        GameObject Instantiate(GameObject prefab);
    }
}