using ExplodingElves.Core;
using ExplodingElves.Views;
using UnityEngine;
using Zenject;

namespace ExplodingElves.Pools
{
    /// <summary>
    ///     Zenject memory pool for ElfView instances.
    ///     SRP: Only manages pooling lifecycle for elves.
    /// </summary>
    public class ElfPool : MonoMemoryPool<Vector3, ElfData, ElfView>
    {
        private const float DefaultSpeed = 3f;

        protected override void Reinitialize(Vector3 position, ElfData data, ElfView item)
        {
            if (item == null)
            {
                Debug.LogError("ElfPool received null item to reinitialize");
                return;
            }

            base.Reinitialize(position, data, item);

            Transform itemTransform = item.transform;
            itemTransform.position = position;
            itemTransform.rotation = Quaternion.identity;

            item.gameObject.SetActive(true);
            item.Initialize(data, data.Speed);
        }

        protected override void OnDespawned(ElfView item)
        {
            base.OnDespawned(item);

            if (item == null) return;

            // Reset physics
            if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            item.gameObject.SetActive(false);
        }
    }
}