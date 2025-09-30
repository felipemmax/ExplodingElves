using System;
using ExplodingElves.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ExplodingElves.Core
{
    public class ElfLogic
    {
        private readonly CollisionHandler _collisionHandler;
        private readonly ElfData _data;
        private float _timeUntilDirectionChange;

        public ElfLogic(ElfData data, CollisionHandler collisionHandler)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _collisionHandler = collisionHandler ?? throw new ArgumentNullException(nameof(collisionHandler));
            ResetDirectionTimer();
        }

        public ElfColor ElfColor => _data.ElfColor;
        public float Speed => _data.Speed;
        public Vector2 DirectionChangeInterval => _data.DirectionChangeInterval;
        
        public bool ShouldChangeDirection => _timeUntilDirectionChange <= 0f;

        public void Tick(float deltaTime)
        {
            _timeUntilDirectionChange -= Mathf.Max(0f, deltaTime);
        }

        public void ResetDirectionTimer()
        {
            _timeUntilDirectionChange = Random.Range(_data.DirectionChangeInterval.x, _data.DirectionChangeInterval.y);
        }

        public CollisionDecision HandleCollision(ElfLogic other)
        {
            if (other == null) return CollisionDecision.None();

            if (other.ElfColor == ElfColor) 
                return _collisionHandler.HandleSameColor(this, other);

            return _collisionHandler.HandleDifferentColor(this, other);
        }
    }
}