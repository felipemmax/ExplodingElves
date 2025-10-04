using System.Reflection;
using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Characters.Collision;
using ExplodingElves.Tests.Mocks;
using NUnit.Framework;
using UnityEngine;

namespace ExplodingElves.Tests
{
    [TestFixture]
    public class ElfCollisionHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _mockCooldownService = new MockSpawnCooldownService();
            _mockSpawnRequestService = new MockSpawnRequestService();
            _mockStrategy = new MockElfCollisionStrategy();

            _handler = new ElfCollisionHandler(_mockCooldownService, _mockSpawnRequestService);
        }

        private ElfCollisionHandler _handler;
        private MockSpawnCooldownService _mockCooldownService;
        private MockSpawnRequestService _mockSpawnRequestService;
        private MockElfCollisionStrategy _mockStrategy;

        [Test]
        public void ProcessCollision_AppliesStunToBothControllers()
        {
            // Arrange
            ElfController controller1 = CreateMockController(ElfColor.Red);
            ElfController controller2 = CreateMockController(ElfColor.Blue);
            _mockStrategy.DecisionToReturn = CollisionDecision.None();

            // Act
            _handler.ProcessCollision(controller1, controller2);

            // Assert
            Assert.IsTrue(controller1.IsStunned);
            Assert.IsTrue(controller2.IsStunned);
        }

        [Test]
        public void ProcessCollision_WhenSpawnExtra_AndCooldownReady_RequestsSpawn()
        {
            // Arrange
            ElfController controller1 = CreateMockController(ElfColor.Red);
            ElfController controller2 = CreateMockController(ElfColor.Red);

            _mockStrategy.DecisionToReturn = CollisionDecision.SpawnExtra();
            _mockCooldownService.CanSpawnResult = true;
            _mockSpawnRequestService.RequestSpawnResult = true;

            // Act
            ElfCollisionHandler.CollisionResult result = _handler.ProcessCollision(controller1, controller2);

            // Assert
            Assert.IsTrue(result.ShouldSpawnExtra);
            Assert.AreEqual(1, _mockSpawnRequestService.RequestSpawnCallCount);
            Assert.AreEqual(ElfColor.Red, _mockSpawnRequestService.LastRequestedColor);
            Assert.AreEqual(1, _mockCooldownService.RegisterSpawnCallCount);
        }

        [Test]
        public void ProcessCollision_WhenSpawnExtra_ButCooldownNotReady_DoesNotSpawn()
        {
            // Arrange
            ElfController controller1 = CreateMockController(ElfColor.Red);
            ElfController controller2 = CreateMockController(ElfColor.Red);

            _mockStrategy.DecisionToReturn = CollisionDecision.SpawnExtra();
            _mockCooldownService.CanSpawnResult = false;
            _mockCooldownService.RemainingCooldown = 1.5f;

            // Act
            ElfCollisionHandler.CollisionResult result = _handler.ProcessCollision(controller1, controller2);

            // Assert
            Assert.IsFalse(result.ShouldSpawnExtra);
            Assert.AreEqual(0, _mockSpawnRequestService.RequestSpawnCallCount);
            Assert.AreEqual(0, _mockCooldownService.RegisterSpawnCallCount);
        }

        [Test]
        public void ProcessCollision_WhenExplodeBoth_ReturnsCorrectResult()
        {
            // Arrange
            ElfController controller1 = CreateMockController(ElfColor.Red);
            ElfController controller2 = CreateMockController(ElfColor.Blue);

            _mockStrategy.DecisionToReturn = CollisionDecision.ExplodeBoth();

            // Act
            ElfCollisionHandler.CollisionResult result = _handler.ProcessCollision(controller1, controller2);

            // Assert
            Assert.IsTrue(result.ShouldExplodeBoth);
            Assert.IsFalse(result.ShouldSpawnExtra);
        }

        private ElfController CreateMockController(ElfColor color)
        {
            ElfData elfData = ScriptableObject.CreateInstance<ElfData>();
            typeof(ElfData).GetField("elfColor", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(elfData, color);

            Elf elf = new(elfData);
            MockNavMeshAgentWrapper agent = new();
            return new ElfController(elf, agent, _mockStrategy);
        }
    }
}