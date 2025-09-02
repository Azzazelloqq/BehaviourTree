using System;
using BehaviourTree.Source;
using BehaviourTree.Source.Nodes;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BehaviourTree.Tests
{
    /// <summary>
    /// Unit tests for behaviour tree nodes.
    /// </summary>
    [TestFixture]
    internal class NodeTests
    {
        #region Selector Node Tests

        /// <summary>
        /// Tests that selector returns success when first child succeeds.
        /// </summary>
        [Test]
        public void SelectorNode_FirstChildSuccess_ReturnsSuccess()
        {
            // Arrange
            var children = new IBehaviourTreeNode[]
            {
                new MockNode(NodeState.Success),
                new MockNode(NodeState.Failure),
                new MockNode(NodeState.Failure)
            };
            var selector = new SelectorNode(children);

            // Act
            var result = selector.Tick();

            // Assert
            Assert.AreEqual(NodeState.Success, result);
            Assert.AreEqual(1, ((MockNode)children[0]).TickCount);
            Assert.AreEqual(0, ((MockNode)children[1]).TickCount);
            Assert.AreEqual(0, ((MockNode)children[2]).TickCount);
        }

        /// <summary>
        /// Tests that selector returns success when middle child succeeds.
        /// </summary>
        [Test]
        public void SelectorNode_MiddleChildSuccess_ReturnsSuccess()
        {
            // Arrange
            var children = new IBehaviourTreeNode[]
            {
                new MockNode(NodeState.Failure),
                new MockNode(NodeState.Success),
                new MockNode(NodeState.Failure)
            };
            var selector = new SelectorNode(children);

            // Act
            var result = selector.Tick();

            // Assert
            Assert.AreEqual(NodeState.Success, result);
            Assert.AreEqual(1, ((MockNode)children[0]).TickCount);
            Assert.AreEqual(1, ((MockNode)children[1]).TickCount);
            Assert.AreEqual(0, ((MockNode)children[2]).TickCount);
        }

        /// <summary>
        /// Tests that selector returns failure when all children fail.
        /// </summary>
        [Test]
        public void SelectorNode_AllChildrenFail_ReturnsFailure()
        {
            // Arrange
            var children = new IBehaviourTreeNode[]
            {
                new MockNode(NodeState.Failure),
                new MockNode(NodeState.Failure),
                new MockNode(NodeState.Failure)
            };
            var selector = new SelectorNode(children);

            // Act
            var result = selector.Tick();

            // Assert
            Assert.AreEqual(NodeState.Failure, result);
            foreach (var child in children)
            {
                Assert.AreEqual(1, ((MockNode)child).TickCount);
            }
        }

        /// <summary>
        /// Tests that selector returns running when child is running.
        /// </summary>
        [Test]
        public void SelectorNode_ChildRunning_ReturnsRunning()
        {
            // Arrange
            var children = new IBehaviourTreeNode[]
            {
                new MockNode(NodeState.Failure),
                new MockNode(NodeState.Running),
                new MockNode(NodeState.Success)
            };
            var selector = new SelectorNode(children);

            // Act
            var result = selector.Tick();

            // Assert
            Assert.AreEqual(NodeState.Running, result);
            Assert.AreEqual(1, ((MockNode)children[0]).TickCount);
            Assert.AreEqual(1, ((MockNode)children[1]).TickCount);
            Assert.AreEqual(0, ((MockNode)children[2]).TickCount);
        }

        /// <summary>
        /// Tests that selector properly disposes all children.
        /// </summary>
        [Test]
        public void SelectorNode_Dispose_DisposesAllChildren()
        {
            // Arrange
            var children = new IBehaviourTreeNode[]
            {
                new MockNode(NodeState.Success),
                new MockNode(NodeState.Success),
                new MockNode(NodeState.Success)
            };
            var selector = new SelectorNode(children);

            // Act
            selector.Dispose();

            // Assert
            foreach (var child in children)
            {
                Assert.IsTrue(((MockNode)child).IsDisposed);
            }
        }

        #endregion

        #region Sequence Node Tests

        /// <summary>
        /// Tests that sequence returns success when all children succeed.
        /// </summary>
        [Test]
        public void SequenceNode_AllChildrenSuccess_ReturnsSuccess()
        {
            // Arrange
            var children = new IBehaviourTreeNode[]
            {
                new MockNode(NodeState.Success),
                new MockNode(NodeState.Success),
                new MockNode(NodeState.Success)
            };
            var sequence = new SequenceNode(children);

            // Act
            var result = sequence.Tick();

            // Assert
            Assert.AreEqual(NodeState.Success, result);
            foreach (var child in children)
            {
                Assert.AreEqual(1, ((MockNode)child).TickCount);
            }
        }

        /// <summary>
        /// Tests that sequence returns failure when first child fails.
        /// </summary>
        [Test]
        public void SequenceNode_FirstChildFails_ReturnsFailure()
        {
            // Arrange
            var children = new IBehaviourTreeNode[]
            {
                new MockNode(NodeState.Failure),
                new MockNode(NodeState.Success),
                new MockNode(NodeState.Success)
            };
            var sequence = new SequenceNode(children);

            // Act
            var result = sequence.Tick();

            // Assert
            Assert.AreEqual(NodeState.Failure, result);
            Assert.AreEqual(1, ((MockNode)children[0]).TickCount);
            Assert.AreEqual(0, ((MockNode)children[1]).TickCount);
            Assert.AreEqual(0, ((MockNode)children[2]).TickCount);
        }

        /// <summary>
        /// Tests that sequence returns failure when middle child fails.
        /// </summary>
        [Test]
        public void SequenceNode_MiddleChildFails_ReturnsFailure()
        {
            // Arrange
            var children = new IBehaviourTreeNode[]
            {
                new MockNode(NodeState.Success),
                new MockNode(NodeState.Failure),
                new MockNode(NodeState.Success)
            };
            var sequence = new SequenceNode(children);

            // Act
            var result = sequence.Tick();

            // Assert
            Assert.AreEqual(NodeState.Failure, result);
            Assert.AreEqual(1, ((MockNode)children[0]).TickCount);
            Assert.AreEqual(1, ((MockNode)children[1]).TickCount);
            Assert.AreEqual(0, ((MockNode)children[2]).TickCount);
        }

        /// <summary>
        /// Tests that sequence returns running when child is running.
        /// </summary>
        [Test]
        public void SequenceNode_ChildRunning_ReturnsRunning()
        {
            // Arrange
            var children = new IBehaviourTreeNode[]
            {
                new MockNode(NodeState.Success),
                new MockNode(NodeState.Running),
                new MockNode(NodeState.Success)
            };
            var sequence = new SequenceNode(children);

            // Act
            var result = sequence.Tick();

            // Assert
            Assert.AreEqual(NodeState.Running, result);
            Assert.AreEqual(1, ((MockNode)children[0]).TickCount);
            Assert.AreEqual(1, ((MockNode)children[1]).TickCount);
            Assert.AreEqual(0, ((MockNode)children[2]).TickCount);
        }

        /// <summary>
        /// Tests that sequence properly disposes all children.
        /// </summary>
        [Test]
        public void SequenceNode_Dispose_DisposesAllChildren()
        {
            // Arrange
            var children = new IBehaviourTreeNode[]
            {
                new MockNode(NodeState.Success),
                new MockNode(NodeState.Success),
                new MockNode(NodeState.Success)
            };
            var sequence = new SequenceNode(children);

            // Act
            sequence.Dispose();

            // Assert
            foreach (var child in children)
            {
                Assert.IsTrue(((MockNode)child).IsDisposed);
            }
        }

        #endregion

        #region Inverter Node Tests

        /// <summary>
        /// Tests that inverter returns failure when child succeeds.
        /// </summary>
        [Test]
        public void InverterNode_ChildSuccess_ReturnsFailure()
        {
            // Arrange
            var child = new MockNode(NodeState.Success);
            var inverter = new InverterNode(child);

            // Act
            var result = inverter.Tick();

            // Assert
            Assert.AreEqual(NodeState.Failure, result);
            Assert.AreEqual(1, child.TickCount);
        }

        /// <summary>
        /// Tests that inverter returns success when child fails.
        /// </summary>
        [Test]
        public void InverterNode_ChildFailure_ReturnsSuccess()
        {
            // Arrange
            var child = new MockNode(NodeState.Failure);
            var inverter = new InverterNode(child);

            // Act
            var result = inverter.Tick();

            // Assert
            Assert.AreEqual(NodeState.Success, result);
            Assert.AreEqual(1, child.TickCount);
        }

        /// <summary>
        /// Tests that inverter returns running when child is running.
        /// </summary>
        [Test]
        public void InverterNode_ChildRunning_ReturnsRunning()
        {
            // Arrange
            var child = new MockNode(NodeState.Running);
            var inverter = new InverterNode(child);

            // Act
            var result = inverter.Tick();

            // Assert
            Assert.AreEqual(NodeState.Running, result);
            Assert.AreEqual(1, child.TickCount);
        }

        /// <summary>
        /// Tests that inverter returns failure for unexpected state.
        /// </summary>
        [Test]
        public void InverterNode_ChildNone_ReturnsFailure()
        {
            // Arrange
            var child = new MockNode(NodeState.None);
            var inverter = new InverterNode(child);

            // Act
            var result = inverter.Tick();

            // Assert
            Assert.AreEqual(NodeState.Failure, result);
            Assert.AreEqual(1, child.TickCount);
        }

        /// <summary>
        /// Tests that inverter properly disposes child.
        /// </summary>
        [Test]
        public void InverterNode_Dispose_DisposesChild()
        {
            // Arrange
            var child = new MockNode(NodeState.Success);
            var inverter = new InverterNode(child);

            // Act
            inverter.Dispose();

            // Assert
            Assert.IsTrue(child.IsDisposed);
        }

        /// <summary>
        /// Tests that inverter correctly exposes child in Children property.
        /// </summary>
        [Test]
        public void InverterNode_Children_ContainsSingleChild()
        {
            // Arrange
            var child = new MockNode(NodeState.Success);
            var inverter = new InverterNode(child);

            // Act
            var children = inverter.Children;

            // Assert
            Assert.AreEqual(1, children.Count);
            Assert.AreEqual(child, children[0]);
        }

        #endregion

        #region Mock Node Implementation

        /// <summary>
        /// Mock node for testing purposes.
        /// </summary>
        private class MockNode : IBehaviourTreeNode
        {
            private readonly NodeState _returnState;
            public int TickCount { get; private set; }
            public bool IsDisposed { get; private set; }

            public MockNode(NodeState returnState)
            {
                _returnState = returnState;
            }

            public NodeState Tick()
            {
                TickCount++;
                return _returnState;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        #endregion
    }
}
