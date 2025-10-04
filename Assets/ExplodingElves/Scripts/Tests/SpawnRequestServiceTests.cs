using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Services;
using ExplodingElves.Interfaces;
using ExplodingElves.Tests.Mocks;
using NUnit.Framework;

namespace ExplodingElves.Tests
{
    [TestFixture]
    public class SpawnRequestServiceTests
    {
        private ISpawnRequestService _service;
        private MockSpawnerRegistryService _mockRegistry;

        [SetUp]
        public void Setup()
        {
            _mockRegistry = new MockSpawnerRegistryService();
            _service = new SpawnRequestService(_mockRegistry);
        }

        [Test]
        public void RequestSpawn_WithRegisteredSpawner_SpawnsAndReturnsTrue()
        {
            // Arrange
            var spawner = new MockSpawner { ElfColor = ElfColor.Red };
            _mockRegistry.Register(spawner);

            // Act
            bool result = _service.RequestSpawn(ElfColor.Red);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, spawner.SpawnCallCount);
        }

        [Test]
        public void RequestSpawn_WithoutRegisteredSpawner_ReturnsFalse()
        {
            // Act
            bool result = _service.RequestSpawn(ElfColor.Blue);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void RequestSpawn_CallsCorrectSpawnerForColor()
        {
            // Arrange
            var redSpawner = new MockSpawner { ElfColor = ElfColor.Red };
            var blueSpawner = new MockSpawner { ElfColor = ElfColor.Blue };
            
            _mockRegistry.Register(redSpawner);
            _mockRegistry.Register(blueSpawner);

            // Act
            _service.RequestSpawn(ElfColor.Red);
            _service.RequestSpawn(ElfColor.Blue);

            // Assert
            Assert.AreEqual(1, redSpawner.SpawnCallCount);
            Assert.AreEqual(1, blueSpawner.SpawnCallCount);
        }
    }
}