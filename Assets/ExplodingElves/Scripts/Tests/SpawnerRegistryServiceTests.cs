using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Services;
using ExplodingElves.Interfaces;
using ExplodingElves.Tests.Mocks;
using NUnit.Framework;

namespace ExplodingElves.Tests
{
    [TestFixture]
    public class SpawnerRegistryServiceTests
    {
        private ISpawnerRegistryService _registry;

        [SetUp]
        public void Setup()
        {
            _registry = new SpawnerRegistryService();
        }

        [Test]
        public void Register_AddsSpawnerToRegistry()
        {
            // Arrange
            var spawner = new MockSpawner { ElfColor = ElfColor.Red };

            // Act
            _registry.Register(spawner);
            var retrieved = _registry.GetSpawner(ElfColor.Red);

            // Assert
            Assert.AreEqual(spawner, retrieved);
        }

        [Test]
        public void Register_SameSpawnerTwice_DoesNotDuplicate()
        {
            // Arrange
            var spawner = new MockSpawner { ElfColor = ElfColor.Red };

            // Act
            _registry.Register(spawner);
            _registry.Register(spawner);

            // Assert
            var retrieved = _registry.GetSpawner(ElfColor.Red);
            Assert.AreEqual(spawner, retrieved);
        }

        [Test]
        public void Unregister_RemovesSpawnerFromRegistry()
        {
            // Arrange
            var spawner = new MockSpawner { ElfColor = ElfColor.Red };
            _registry.Register(spawner);

            // Act
            _registry.Unregister(spawner);
            var retrieved = _registry.GetSpawner(ElfColor.Red);

            // Assert
            Assert.IsNull(retrieved);
        }

        [Test]
        public void GetSpawner_ReturnsNullForUnregisteredColor()
        {
            // Act
            var spawner = _registry.GetSpawner(ElfColor.Blue);

            // Assert
            Assert.IsNull(spawner);
        }

        [Test]
        public void GetSpawner_ReturnsCorrectSpawnerForEachColor()
        {
            // Arrange
            var redSpawner = new MockSpawner { ElfColor = ElfColor.Red };
            var blueSpawner = new MockSpawner { ElfColor = ElfColor.Blue };
            var whiteSpawner = new MockSpawner { ElfColor = ElfColor.White };

            _registry.Register(redSpawner);
            _registry.Register(blueSpawner);
            _registry.Register(whiteSpawner);

            // Act & Assert
            Assert.AreEqual(redSpawner, _registry.GetSpawner(ElfColor.Red));
            Assert.AreEqual(blueSpawner, _registry.GetSpawner(ElfColor.Blue));
            Assert.AreEqual(whiteSpawner, _registry.GetSpawner(ElfColor.White));
        }
    }
}