using ExplodingElves.Core.Characters.Services;
using ExplodingElves.Core.Services;
using ExplodingElves.Interfaces;
using NUnit.Framework;

namespace ExplodingElves.Tests
{
    [TestFixture]
    public class SpawnCooldownServiceTests
    {
        [SetUp]
        public void Setup()
        {
            _service = new SpawnGlobalCooldownService(CooldownDuration);
        }

        private ISpawnCooldownService _service;
        private const float CooldownDuration = 3f;

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
            SpawnGlobalCooldownService service = new(-5f);

            // Act
            service.RegisterSpawn();
            bool canSpawn = service.CanSpawn();

            // Assert - With 0 cooldown, should be able to spawn immediately
            Assert.IsTrue(canSpawn);
        }
    }
}