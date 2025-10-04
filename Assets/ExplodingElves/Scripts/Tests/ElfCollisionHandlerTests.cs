using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Collision;
using ExplodingElves.Tests.Mocks;
using NUnit.Framework;
using UnityEngine;

namespace ExplodingElves.Tests
{
    [TestFixture]
    public class ElfCollisionHandlerTests
    {
        private ElfCollisionHandler _handler;
        private MockSpawnCooldownService _mockCooldownService;
        private MockSpawnRequestService _mockSpawnRequestService;
        private MockElfCollisionStrategy _mockStrategy;

        [SetUp]
        public void Setup()
        {
            _mockCooldownService = new MockSpawnCooldownService();
            _mockSpawnRequestService = new MockSpawnRequestService();
            _mockStrategy = new MockElfCollisionStrategy();
            
            _handler = new ElfCollisionHandler(_mockCooldownService, _mockSpawnRequestService);
        }

        [Test]
        public void ProcessCollision_AppliesStunToBothControllers()
        {
            // Arrange
            var controller1 = CreateMockController(ElfColor.Red);
            var controller2 = CreateMockController(ElfColor.Blue);
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
            var controller1 = CreateMockController(ElfColor.Red);
            var controller2 = CreateMockController(ElfColor.Red);
            
            _mockStrategy.DecisionToReturn = CollisionDecision.SpawnExtra();
            _mockCooldownService.CanSpawnResult = true;
            _mockSpawnRequestService.RequestSpawnResult = true;

            // Act
            var result = _handler.ProcessCollision(controller1, controller2);

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
            var controller1 = CreateMockController(ElfColor.Red);
            var controller2 = CreateMockController(ElfColor.Red);
            
            _mockStrategy.DecisionToReturn = CollisionDecision.SpawnExtra();
            _mockCooldownService.CanSpawnResult = false;
            _mockCooldownService.RemainingCooldown = 1.5f;

            // Act
            var result = _handler.ProcessCollision(controller1, controller2);

            // Assert
            Assert.IsFalse(result.ShouldSpawnExtra);
            Assert.AreEqual(0, _mockSpawnRequestService.RequestSpawnCallCount);
            Assert.AreEqual(0, _mockCooldownService.RegisterSpawnCallCount);
        }

        [Test]
        public void ProcessCollision_WhenExplodeBoth_ReturnsCorrectResult()
        {
            // Arrange
            var controller1 = CreateMockController(ElfColor.Red);
            var controller2 = CreateMockController(ElfColor.Blue);
            
            _mockStrategy.DecisionToReturn = CollisionDecision.ExplodeBoth();

            // Act
            var result = _handler.ProcessCollision(controller1, controller2);

            // Assert
            Assert.IsTrue(result.ShouldExplodeBoth);
            Assert.IsFalse(result.ShouldSpawnExtra);
        }

        private ElfController CreateMockController(ElfColor color)
        {
            var elfData = ScriptableObject.CreateInstance<ElfData>();
            typeof(ElfData).GetField("elfColor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(elfData, color);
            
            var elf = new Elf(elfData);
            var agent = new MockNavMeshAgentWrapper();
            return new ElfController(elf, agent, _mockStrategy);
        }
    }
}