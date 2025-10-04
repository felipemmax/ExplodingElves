using ExplodingElves.Core.Characters;
using ExplodingElves.Core.Characters.States;
using ExplodingElves.Tests.Mocks;
using NUnit.Framework;
using UnityEngine;

namespace ExplodingElves.Tests
{
    [TestFixture]
    public class ElfStateTests
    {
        private ElfStateManager _stateManager;
        private Elf _elf;
        private ElfMovementBehavior _movement;
        private MockNavMeshAgentWrapper _mockAgent;
        private ElfData _mockElfData;

        [SetUp]
        public void Setup()
        {
            _mockElfData = ScriptableObject.CreateInstance<ElfData>();
            _elf = new Elf(_mockElfData);
            _mockElfData = ScriptableObject.CreateInstance<ElfData>();
            _elf = new Elf(_mockElfData);
            _mockAgent = new MockNavMeshAgentWrapper();
            _movement = new ElfMovementBehavior(_mockAgent, Vector2.one);
            _stateManager = new ElfStateManager(_elf, _movement);
        }

        [TearDown]
        public void TearDown()
        {
            if (_mockElfData != null)
                Object.DestroyImmediate(_mockElfData);
            
            _mockAgent?.Destroy();
        }

        [Test]
        public void NormalState_EnterResumesMovement()
        {
            // Arrange
            var state = new NormalState(_stateManager);

            // Act
            state.Enter();

            // Assert
            Assert.IsFalse(_mockAgent.isStopped);
        }

        [Test]
        public void NormalState_ApplyStunTransitionsToStunnedState()
        {
            // Arrange
            _stateManager.TransitionToState(new NormalState(_stateManager));

            // Act
            _stateManager.ApplyStun();

            // Assert
            Assert.IsTrue(_stateManager.IsStunned);
        }

        [Test]
        public void StunnedState_EnterStopsMovement()
        {
            // Arrange
            var state = new StunnedState(_stateManager);

            // Act
            state.Enter();

            // Assert
            Assert.IsTrue(_mockAgent.isStopped);
            Assert.AreEqual(1, _mockAgent.ResetPathCallCount);
        }

        [Test]
        public void StunnedState_UpdateTransitionsToNormalAfterDuration()
        {
            // Arrange
            _stateManager.TransitionToState(new StunnedState(_stateManager));

            // Act
            _stateManager.Update(0.6f); // Stun duration is 0.5f

            // Assert
            Assert.IsFalse(_stateManager.IsStunned);
        }

        [Test]
        public void StunnedState_ApplyStunResetsTimer()
        {
            // Arrange
            _stateManager.TransitionToState(new StunnedState(_stateManager));
            _stateManager.Update(0.4f);

            // Act - Apply stun again
            _stateManager.ApplyStun();
            _stateManager.Update(0.4f);

            // Assert - Should still be stunned because timer was reset
            Assert.IsTrue(_stateManager.IsStunned);
        }

        [Test]
        public void DeadState_CannotBeStunned()
        {
            // Arrange
            _stateManager.TransitionToState(new DeadState(_stateManager));

            // Act
            _stateManager.ApplyStun();

            // Assert
            Assert.IsTrue(_stateManager.IsDead);
            Assert.IsFalse(_stateManager.IsStunned);
        }

        [Test]
        public void Kill_TransitionsToDeadState()
        {
            // Arrange
            _stateManager.TransitionToState(new NormalState(_stateManager));

            // Act
            _stateManager.Kill();

            // Assert
            Assert.IsTrue(_stateManager.IsDead);
        }

        [Test]
        public void StateManager_ThrowsExceptionWhenElfIsNull()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => new ElfStateManager(null, _movement));
        }

        [Test]
        public void StateManager_ThrowsExceptionWhenMovementIsNull()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => new ElfStateManager(_elf, null));
        }
    }
}