using UnityEngine;

namespace ExplodingElves.Interfaces
{
    public interface IVFXService
    {
        void PlayExplosion(Vector3 position);

        void PlaySpawnVFX(Vector3 position);

        void PlayCollisionVFX(Vector3 position);
    }
}