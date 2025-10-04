using System;
using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Characters.Collision;
using ExplodingElves.Tests.Mocks;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ExplodingElves.Tests
{
    [TestFixture]
    public class ElfControllerTests
    {
        [SetUp]
        public void Setup()
        {
            _elfData = ScriptableObject.CreateInstance<ElfData>();
            _elf = new Elf(_elfData);
            _mockAgent = new MockNavMeshAgentWrapper();
            _mockStrategy = new MockElfCollisionStrategy();
            _controller = new ElfController(_elf, _mockAgent, _mockStrategy);
        }

        [TearDown]
        public void TearDown()
        {
            if (_elfData != null)
                Object.DestroyImmediate(_elfData);

            _mockAgent?.Destroy();
        }

        private ElfController _controller;
        private Elf _elf;
        private MockNavMeshAgentWrapper _mockAgent;
        private MockElfCollisionStrategy _mockStrategy;
        private ElfData _elfData;

        [Test]
        public void Constructor_ThrowsExceptionWhenElfIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ElfController(null, _mockAgent, _mockStrategy));
        }

        [Test]
        public void Constructor_ThrowsExceptionWhenAgentIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ElfController(_elf, null, _mockStrategy));
        }

        [Test]
        public void Constructor_ThrowsExceptionWhenStrategyIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ElfController(_elf, _mockAgent, null));
        }

        [Test]
        public void ApplyStun_TransitionsToStunnedState()
        {
            // Act
            _controller.ApplyStun();

            // Assert
            Assert.IsTrue(_controller.IsStunned);
        }

        [Test]
        public void Kill_TransitionsToDeadState()
        {
            // Act
            _controller.Kill();

            // Assert
            Assert.IsTrue(_controller.IsDead);
        }

        [Test]
        public void HandleCollisionWith_WhenDead_ReturnsNone()
        {
            // Arrange
            ElfController otherController = CreateController();
            _controller.Kill();

            // Act
            CollisionDecision decision = _controller.HandleCollisionWith(otherController);

            // Assert
            Assert.IsFalse(decision.ShouldSpawnExtra);
            Assert.IsFalse(decision.ShouldExplodeBoth);
        }

        [Test]
        public void HandleCollisionWith_WhenOtherIsDead_ReturnsNone()
        {
            // Arrange
            ElfController otherController = CreateController();
            otherController.Kill();

            // Act
            CollisionDecision decision = _controller.HandleCollisionWith(otherController);

            // Assert
            Assert.IsFalse(decision.ShouldSpawnExtra);
            Assert.IsFalse(decision.ShouldExplodeBoth);
        }

        [Test]
        public void HandleCollisionWith_WhenOtherIsNull_ReturnsNone()
        {
            // Act
            CollisionDecision decision = _controller.HandleCollisionWith(null);

            // Assert
            Assert.IsFalse(decision.ShouldSpawnExtra);
            Assert.IsFalse(decision.ShouldExplodeBoth);
        }

        [Test]
        public void HandleCollisionWith_DelegatesToStrategy()
        {
            // Arrange
            ElfController otherController = CreateController();
            CollisionDecision expectedDecision = CollisionDecision.SpawnExtra();
            _mockStrategy.DecisionToReturn = expectedDecision;

            // Act
            CollisionDecision decision = _controller.HandleCollisionWith(otherController);

            // Assert
            Assert.AreEqual(1, _mockStrategy.DecideCallCount);
            Assert.AreEqual(expectedDecision.ShouldSpawnExtra, decision.ShouldSpawnExtra);
            Assert.AreEqual(expectedDecision.ShouldExplodeBoth, decision.ShouldExplodeBoth);
        }

        private ElfController CreateController()
        {
            ElfData elfData = ScriptableObject.CreateInstance<ElfData>();
            Elf elf = new(elfData);
            MockNavMeshAgentWrapper agent = new();
            return new ElfController(elf, agent, _mockStrategy);
        }
    }
}