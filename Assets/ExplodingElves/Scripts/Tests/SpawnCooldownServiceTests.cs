using ExplodingElves.Core.Services;
using ExplodingElves.Interfaces;
using NUnit.Framework;
using UnityEngine;

namespace ExplodingElves.Tests
{
    [TestFixture]
    public class SpawnCooldownServiceTests
    {
        private ISpawnCooldownService _service;
        private const float CooldownDuration = 3f;

        [SetUp]
        public void Setup()
        {
            _service = new SpawnGlobalCooldownService(CooldownDuration);
        }

        [Test]
        public void CanSpawn_InitiallyReturnsTrue()
        {
            // Act
            bool canSpawn = _service.CanSpawn();

            // Assert
            Assert.IsTrue(canSpawn);
        }

        [Test]
        public void RegisterSpawn_PreventsFutureSpawnsUntilCooldown()
        {
            // Act
            _service.RegisterSpawn();
            bool canSpawnImmediately = _service.CanSpawn();

            // Assert
            Assert.IsFalse(canSpawnImmediately);
        }

        [Test]
        public void GetRemainingCooldown_ReturnsCorrectValue()
        {
            // Act
            _service.RegisterSpawn();
            float remaining = _service.GetRemainingCooldown();

            // Assert
            Assert.That(remaining, Is.GreaterThan(0).And.LessThanOrEqualTo(CooldownDuration));
        }

        [Test]
        public void GetRemainingCooldown_ReturnsZeroWhenNotOnCooldown()
        {
            // Act
            float remaining = _service.GetRemainingCooldown();

            // Assert
            Assert.That(remaining, Is.EqualTo(0).Within(0.01f));
        }

        [Test]
        public void NegativeCooldownDuration_TreatedAsZero()
        {
            // Arrange
            var service = new SpawnGlobalCooldownService(-5f);

            // Act
            service.RegisterSpawn();
            bool canSpawn = service.CanSpawn();

            // Assert - With 0 cooldown, should be able to spawn immediately
            Assert.IsTrue(canSpawn);
        }
    }
}