using System;
using System.Collections.Generic;
using BehaviourTree.Source;
using BehaviourTree.Source.Logger;
using BehaviourTree.Source.Nodes;
using NUnit.Framework;

namespace BehaviourTree.Tests
{
    /// <summary>
    /// Integration tests for complex behaviour tree scenarios.
    /// </summary>
    [TestFixture]
    internal class IntegrationTests
    {
        /// <summary>
        /// Tests a complex tree with multiple levels of nesting.
        /// </summary>
        [Test]
        public void ComplexTree_MultiLevelNesting_ExecutesCorrectly()
        {
            // Arrange
            var tree = BuildComplexTree();

            // Act
            var result = tree.Tick();

            // Assert
            Assert.AreEqual(NodeState.Success, result);
        }

        /// <summary>
        /// Tests that a tree with running nodes maintains state correctly.
        /// </summary>
        [Test]
        public void BehaviourTree_RunningNodes_MaintainsState()
        {
            // Arrange
            var runningNode = new StatefulNode();
            var tree = new SequenceNode(new IBehaviourTreeNode[]
            {
                new SuccessNode(),
                runningNode,
                new SuccessNode()
            });

            // Act & Assert
            Assert.AreEqual(NodeState.Running, tree.Tick());
            Assert.AreEqual(NodeState.Running, tree.Tick());
            Assert.AreEqual(NodeState.Success, tree.Tick()); // StatefulNode completes on third tick
        }

        /// <summary>
        /// Tests selector fallback behavior.
        /// </summary>
        [Test]
        public void SelectorTree_FallbackBehavior_WorksCorrectly()
        {
            // Arrange
            var attemptCount = 0;
            var tree = new SelectorNode(new IBehaviourTreeNode[]
            {
                new ConditionalNode(() => attemptCount++ > 2), // Fails first 3 times
                new SuccessNode() // Fallback
            });

            // Act & Assert
            Assert.AreEqual(NodeState.Success, tree.Tick()); // Fallback used
            Assert.AreEqual(NodeState.Success, tree.Tick()); // Fallback used
            Assert.AreEqual(NodeState.Success, tree.Tick()); // Fallback used
            Assert.AreEqual(NodeState.Success, tree.Tick()); // Primary succeeds
            Assert.AreEqual(4, attemptCount);
        }

        /// <summary>
        /// Tests complex conditional logic with inverters.
        /// </summary>
        [Test]
        public void ComplexConditionalTree_WithInverters_ExecutesCorrectly()
        {
            // Arrange
            var isEnemyVisible = false;
            var hasAmmo = true;
            
            var tree = new SelectorNode(new IBehaviourTreeNode[]
            {
                // Attack if enemy visible and has ammo
                new SequenceNode(new IBehaviourTreeNode[]
                {
                    new ConditionalNode(() => isEnemyVisible),
                    new ConditionalNode(() => hasAmmo),
                    new ActionNode("Attack")
                }),
                
                // Reload if no ammo
                new SequenceNode(new IBehaviourTreeNode[]
                {
                    new InverterNode(new ConditionalNode(() => hasAmmo)),
                    new ActionNode("Reload")
                }),
                
                // Default: Patrol
                new ActionNode("Patrol")
            });

            // Act & Assert
            // No enemy, has ammo -> Patrol
            Assert.AreEqual(NodeState.Success, tree.Tick());
            
            // Enemy visible, has ammo -> Attack
            isEnemyVisible = true;
            Assert.AreEqual(NodeState.Success, tree.Tick());
            
            // Enemy visible, no ammo -> Reload
            hasAmmo = false;
            Assert.AreEqual(NodeState.Success, tree.Tick());
        }

        /// <summary>
        /// Tests that all nodes are properly disposed in complex tree.
        /// </summary>
        [Test]
        public void ComplexTree_Dispose_DisposesAllNodes()
        {
            // Arrange
            var disposableNodes = new List<DisposableNode>();
            
            for (int i = 0; i < 5; i++)
            {
                disposableNodes.Add(new DisposableNode());
            }

            var tree = new SelectorNode(new IBehaviourTreeNode[]
            {
                new SequenceNode(new IBehaviourTreeNode[]
                {
                    disposableNodes[0],
                    disposableNodes[1]
                }),
                new InverterNode(disposableNodes[2]),
                new SelectorNode(new IBehaviourTreeNode[]
                {
                    disposableNodes[3],
                    disposableNodes[4]
                })
            });

            // Act
            tree.Dispose();

            // Assert
            foreach (var node in disposableNodes)
            {
                Assert.IsTrue(node.IsDisposed, $"Node {disposableNodes.IndexOf(node)} was not disposed");
            }
        }

        /// <summary>
        /// Tests behaviour tree with logger integration.
        /// </summary>
        [Test]
        public void BehaviourTree_WithLogger_LogsAllNodeExecutions()
        {
            // Arrange
            var logs = new List<string>();
            var loggerSettings = new LoggerSettings("[TEST]", "", log => logs.Add(log));
            var logger = new BehaviourTreeLogger(loggerSettings);
            
            var tree = new SequenceNode(new IBehaviourTreeNode[]
            {
                new SuccessNode(),
                new SuccessNode(),
                new SuccessNode()
            });
            
            var loggedTree = logger.WrapWithLogging(tree);

            // Act
            loggedTree.Tick();

            // Assert
            Assert.AreEqual(4, logs.Count); // 3 children + 1 sequence
            foreach (var log in logs)
            {
                StringAssert.Contains("[TEST]", log);
                StringAssert.Contains("Success", log);
            }
        }

        /// <summary>
        /// Tests parallel-like behavior simulation.
        /// </summary>
        [Test]
        public void ParallelBehaviorSimulation_MultipleBranches_ExecuteIndependently()
        {
            // Arrange
            var branch1Executed = false;
            var branch2Executed = false;
            var branch3Executed = false;
            
            // Simulate parallel execution with selector that tries all branches
            var tree = new ParallelSimulatorNode(new IBehaviourTreeNode[]
            {
                new ActionNode("Branch1", () => branch1Executed = true),
                new ActionNode("Branch2", () => branch2Executed = true),
                new ActionNode("Branch3", () => branch3Executed = true)
            });

            // Act
            tree.Tick();

            // Assert
            Assert.IsTrue(branch1Executed);
            Assert.IsTrue(branch2Executed);
            Assert.IsTrue(branch3Executed);
        }

        /// <summary>
        /// Tests memory leak prevention with circular references.
        /// </summary>
        [Test]
        public void BehaviourTree_CircularReferences_ProperlyDisposed()
        {
            // Arrange
            var node1 = new ReferenceNode();
            var node2 = new ReferenceNode();
            var node3 = new ReferenceNode();
            
            // Create circular references
            node1.SetReference(node2);
            node2.SetReference(node3);
            node3.SetReference(node1);
            
            var tree = new SequenceNode(new IBehaviourTreeNode[] { node1, node2, node3 });

            // Act
            tree.Dispose();

            // Assert
            Assert.IsTrue(node1.IsDisposed);
            Assert.IsTrue(node2.IsDisposed);
            Assert.IsTrue(node3.IsDisposed);
        }

        #region Helper Methods

        private IBehaviourTreeNode BuildComplexTree()
        {
            return new SelectorNode(new IBehaviourTreeNode[]
            {
                new SequenceNode(new IBehaviourTreeNode[]
                {
                    new InverterNode(new FailureNode()),
                    new SuccessNode(),
                    new SelectorNode(new IBehaviourTreeNode[]
                    {
                        new FailureNode(),
                        new SuccessNode()
                    })
                }),
                new FailureNode()
            });
        }

        #endregion

        #region Test Node Implementations

        private class SuccessNode : IBehaviourTreeNode
        {
            public NodeState Tick() => NodeState.Success;
            public void Dispose() { }
        }

        private class FailureNode : IBehaviourTreeNode
        {
            public NodeState Tick() => NodeState.Failure;
            public void Dispose() { }
        }

        private class StatefulNode : IBehaviourTreeNode
        {
            private int _tickCount = 0;

            public NodeState Tick()
            {
                _tickCount++;
                return _tickCount < 3 ? NodeState.Running : NodeState.Success;
            }

            public void Dispose() { }
        }

        private class ConditionalNode : IBehaviourTreeNode
        {
            private readonly Func<bool> _condition;

            public ConditionalNode(Func<bool> condition)
            {
                _condition = condition;
            }

            public NodeState Tick()
            {
                return _condition() ? NodeState.Success : NodeState.Failure;
            }

            public void Dispose() { }
        }

        private class ActionNode : IBehaviourTreeNode
        {
            private readonly string _name;
            private readonly Action _action;

            public ActionNode(string name, Action action = null)
            {
                _name = name;
                _action = action;
            }

            public NodeState Tick()
            {
                _action?.Invoke();
                return NodeState.Success;
            }

            public void Dispose() { }
        }

        private class DisposableNode : IBehaviourTreeNode
        {
            public bool IsDisposed { get; private set; }

            public NodeState Tick() => NodeState.Success;

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        private class ParallelSimulatorNode : IBehaviourTreeNode
        {
            private readonly IBehaviourTreeNode[] _children;

            public ParallelSimulatorNode(IBehaviourTreeNode[] children)
            {
                _children = children;
            }

            public NodeState Tick()
            {
                var anyRunning = false;
                var anyFailed = false;

                foreach (var child in _children)
                {
                    var state = child.Tick();
                    if (state == NodeState.Running)
                        anyRunning = true;
                    else if (state == NodeState.Failure)
                        anyFailed = true;
                }

                if (anyRunning)
                    return NodeState.Running;
                
                return anyFailed ? NodeState.Failure : NodeState.Success;
            }

            public void Dispose()
            {
                foreach (var child in _children)
                {
                    child.Dispose();
                }
            }
        }

        private class ReferenceNode : IBehaviourTreeNode
        {
            private ReferenceNode _reference;
            public bool IsDisposed { get; private set; }

            public void SetReference(ReferenceNode reference)
            {
                _reference = reference;
            }

            public NodeState Tick() => NodeState.Success;

            public void Dispose()
            {
                IsDisposed = true;
                _reference = null;
            }
        }

        #endregion
    }
}


