using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Collision;
using ExplodingElves.Interfaces;
using NUnit.Framework;
using UnityEngine;

namespace ExplodingElves.Tests
{
    [TestFixture]
    public class CollisionStrategyTests
    {
        private IElfCollisionStrategy _strategy;
        private ElfData _redElfData;
        private ElfData _blueElfData;

        [SetUp]
        public void Setup()
        {
            _strategy = new ColorBasedCollisionStrategy();
            
            _redElfData = ScriptableObject.CreateInstance<ElfData>();
            typeof(ElfData).GetField("elfColor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_redElfData, ElfColor.Red);
            
            _blueElfData = ScriptableObject.CreateInstance<ElfData>();
            typeof(ElfData).GetField("elfColor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_blueElfData, ElfColor.Blue);
        }

        [TearDown]
        public void TearDown()
        {
            if (_redElfData != null)
                Object.DestroyImmediate(_redElfData);
            if (_blueElfData != null)
                Object.DestroyImmediate(_blueElfData);
        }

        [Test]
        public void SameColorCollision_ShouldSpawnExtra()
        {
            // Arrange
            var elf1 = new Elf(_redElfData);
            var elf2 = new Elf(_redElfData);

            // Act
            CollisionDecision decision = _strategy.Decide(elf1, elf2);

            // Assert
            Assert.IsTrue(decision.ShouldSpawnExtra);
            Assert.IsFalse(decision.ShouldExplodeBoth);
        }

        [Test]
        public void DifferentColorCollision_ShouldExplodeBoth()
        {
            // Arrange
            var elf1 = new Elf(_redElfData);
            var elf2 = new Elf(_blueElfData);

            // Act
            CollisionDecision decision = _strategy.Decide(elf1, elf2);

            // Assert
            Assert.IsFalse(decision.ShouldSpawnExtra);
            Assert.IsTrue(decision.ShouldExplodeBoth);
        }

        [Test]
        public void NullElf_ShouldReturnNoneDecision()
        {
            // Arrange
            var elf = new Elf(_redElfData);

            // Act
            CollisionDecision decision = _strategy.Decide(null, elf);

            // Assert
            Assert.IsFalse(decision.ShouldSpawnExtra);
            Assert.IsFalse(decision.ShouldExplodeBoth);
        }

        [Test]
        public void BothElfsNull_ShouldReturnNoneDecision()
        {
            // Act
            CollisionDecision decision = _strategy.Decide(null, null);

            // Assert
            Assert.IsFalse(decision.ShouldSpawnExtra);
            Assert.IsFalse(decision.ShouldExplodeBoth);
        }
    }
}