using ExplodingElves.Core.Pooling;
using ExplodingElves.Interfaces;
using ExplodingElves.Tests.Mocks;
using NUnit.Framework;
using UnityEngine;

namespace ExplodingElves.Tests
{
    [TestFixture]
    public class PrefabPoolTests
    {
        [SetUp]
        public void Setup()
        {
            _instantiator = new MockGameObjectInstantiator();
            _pool = new PrefabPool(_instantiator);

            _testPrefab = new GameObject("TestPrefab");
        }

        [TearDown]
        public void TearDown()
        {
            if (_testPrefab != null)
                Object.DestroyImmediate(_testPrefab);
        }

        private IPrefabPool _pool;
        private MockGameObjectInstantiator _instantiator;
        private GameObject _testPrefab;

        [Test]
        public void Spawn_WithNullPrefab_ReturnsNull()
        {
            // Act
            GameObject instance = _pool.Spawn(null, Vector3.zero, Quaternion.identity);

            // Assert
            Assert.IsNull(instance);
        }

        [Test]
        public void Spawn_CreatesNewInstanceWhenPoolEmpty()
        {
            // Arrange
            GameObject newInstance = new("Instance");
            _instantiator.SetResult(_testPrefab, newInstance);

            // Act
            GameObject instance = _pool.Spawn(_testPrefab, Vector3.zero, Quaternion.identity);

            // Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual(1, _instantiator.InstantiateCallCount);
        }

        [Test]
        public void Despawn_WithNullInstance_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _pool.Despawn(null));
        }

        [Test]
        public void Warmup_CreatesSpecifiedNumberOfInstances()
        {
            // Arrange
            int warmupCount = 5;
            _instantiator.SetResult(_testPrefab, () => new GameObject("WarmupInstance"));

            // Act
            _pool.Warmup(_testPrefab, warmupCount);

            // Assert
            Assert.AreEqual(warmupCount, _instantiator.InstantiateCallCount);
        }

        [Test]
        public void Warmup_WithZeroCount_DoesNothing()
        {
            // Act
            _pool.Warmup(_testPrefab, 0);

            // Assert
            Assert.AreEqual(0, _instantiator.InstantiateCallCount);
        }

        [Test]
        public void Warmup_WithNullPrefab_DoesNothing()
        {
            // Act
            _pool.Warmup(null, 5);

            // Assert
            Assert.AreEqual(0, _instantiator.InstantiateCallCount);
        }
    }
}