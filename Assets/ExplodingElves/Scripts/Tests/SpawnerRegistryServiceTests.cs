using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Services;
using ExplodingElves.Core.Spawners;
using ExplodingElves.Core.Spawners.Services;
using ExplodingElves.Interfaces;
using ExplodingElves.Tests.Mocks;
using NUnit.Framework;

namespace ExplodingElves.Tests
{
    [TestFixture]
    public class SpawnerRegistryServiceTests
    {
        [SetUp]
        public void Setup()
        {
            _registry = new SpawnerRegistryService();
        }

        private ISpawnerRegistryService _registry;

        [Test]
        public void Register_AddsSpawnerToRegistry()
        {
            // Arrange
            MockSpawner spawner = new() { ElfColor = ElfColor.Red };

            // Act
            _registry.Register(spawner);
            ISpawner retrieved = _registry.GetSpawner(ElfColor.Red);

            // Assert
            Assert.AreEqual(spawner, retrieved);
        }

        [Test]
        public void Register_SameSpawnerTwice_DoesNotDuplicate()
        {
            // Arrange
            MockSpawner spawner = new() { ElfColor = ElfColor.Red };

            // Act
            _registry.Register(spawner);
            _registry.Register(spawner);

            // Assert
            ISpawner retrieved = _registry.GetSpawner(ElfColor.Red);
            Assert.AreEqual(spawner, retrieved);
        }

        [Test]
        public void Unregister_RemovesSpawnerFromRegistry()
        {
            // Arrange
            MockSpawner spawner = new() { ElfColor = ElfColor.Red };
            _registry.Register(spawner);

            // Act
            _registry.Unregister(spawner);
            ISpawner retrieved = _registry.GetSpawner(ElfColor.Red);

            // Assert
            Assert.IsNull(retrieved);
        }

        [Test]
        public void GetSpawner_ReturnsNullForUnregisteredColor()
        {
            // Act
            ISpawner spawner = _registry.GetSpawner(ElfColor.Blue);

            // Assert
            Assert.IsNull(spawner);
        }

        [Test]
        public void GetSpawner_ReturnsCorrectSpawnerForEachColor()
        {
            // Arrange
            MockSpawner redSpawner = new() { ElfColor = ElfColor.Red };
            MockSpawner blueSpawner = new() { ElfColor = ElfColor.Blue };
            MockSpawner whiteSpawner = new() { ElfColor = ElfColor.White };

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