using System;
using System.Collections.Generic;
using BehaviourTree.Source;
using BehaviourTree.Source.Logger;
using BehaviourTree.Source.Nodes;
using NUnit.Framework;

namespace BehaviourTree.Tests
{
    /// <summary>
    /// Unit tests for behaviour tree logger functionality.
    /// </summary>
    [TestFixture]
    internal class LoggerTests
    {
        private List<string> _logMessages;
        private Action<string> _logAction;

        [SetUp]
        public void SetUp()
        {
            _logMessages = new List<string>();
            _logAction = message => _logMessages.Add(message);
        }

        /// <summary>
        /// Tests that logger wraps single node correctly.
        /// </summary>
        [Test]
        public void BehaviourTreeLogger_WrapSingleNode_LogsCorrectly()
        {
            // Arrange
            var settings = new LoggerSettings("[TEST]", "", _logAction);
            var logger = new BehaviourTreeLogger(settings);
            var node = new TestActionNode(NodeState.Success);

            // Act
            var wrappedNode = logger.WrapWithLogging(node);
            var result = wrappedNode.Tick();

            // Assert
            Assert.AreEqual(NodeState.Success, result);
            Assert.AreEqual(1, _logMessages.Count);
            StringAssert.Contains("TestActionNode", _logMessages[0]);
            StringAssert.Contains("Success", _logMessages[0]);
            StringAssert.Contains("[TEST]", _logMessages[0]);
        }

        /// <summary>
        /// Tests that logger includes prefix and postfix in log messages.
        /// </summary>
        [Test]
        public void LoggerSettings_WithPrefixAndPostfix_IncludedInLogs()
        {
            // Arrange
            var settings = new LoggerSettings("[PREFIX]", "[POSTFIX]", _logAction);
            var logger = new BehaviourTreeLogger(settings);
            var node = new TestActionNode(NodeState.Failure);

            // Act
            var wrappedNode = logger.WrapWithLogging(node);
            wrappedNode.Tick();

            // Assert
            Assert.AreEqual(1, _logMessages.Count);
            StringAssert.Contains("[PREFIX]", _logMessages[0]);
            StringAssert.Contains("[POSTFIX]", _logMessages[0]);
            StringAssert.Contains("Failure", _logMessages[0]);
        }

        /// <summary>
        /// Tests that logger only logs state changes, not repeated states.
        /// </summary>
        [Test]
        public void LoggingNodeDecorator_RepeatedState_LogsOnlyOnce()
        {
            // Arrange
            var settings = new LoggerSettings(_logAction);
            var node = new TestActionNode(NodeState.Running);
            var decorator = new LoggingNodeDecorator(node, settings);

            // Act
            decorator.Tick(); // First tick - should log
            decorator.Tick(); // Second tick - same state, should not log
            decorator.Tick(); // Third tick - same state, should not log

            // Assert
            Assert.AreEqual(1, _logMessages.Count);
        }

        /// <summary>
        /// Tests that logger logs state changes.
        /// </summary>
        [Test]
        public void LoggingNodeDecorator_StateChanges_LogsEachChange()
        {
            // Arrange
            var settings = new LoggerSettings(_logAction);
            var node = new TestSequentialNode();
            var decorator = new LoggingNodeDecorator(node, settings);

            // Act
            decorator.Tick(); // Returns Running
            decorator.Tick(); // Returns Success
            decorator.Tick(); // Returns Failure

            // Assert
            Assert.AreEqual(3, _logMessages.Count);
            StringAssert.Contains("Running", _logMessages[0]);
            StringAssert.Contains("Success", _logMessages[1]);
            StringAssert.Contains("Failure", _logMessages[2]);
        }

        /// <summary>
        /// Tests that logger wraps composite nodes recursively.
        /// </summary>
        [Test]
        public void BehaviourTreeLogger_WrapCompositeNode_WrapsAllChildren()
        {
            // Arrange
            var settings = new LoggerSettings("[BT]", _logAction);
            var logger = new BehaviourTreeLogger(settings);
            
            var children = new IBehaviourTreeNode[]
            {
                new TestActionNode(NodeState.Success),
                new TestActionNode(NodeState.Success)
            };
            var sequence = new SequenceNode(children);

            // Act
            var wrappedNode = logger.WrapWithLogging(sequence);
            wrappedNode.Tick();

            // Assert
            // Should log: child1, child2, sequence
            Assert.AreEqual(3, _logMessages.Count);
            StringAssert.Contains("TestActionNode", _logMessages[0]);
            StringAssert.Contains("TestActionNode", _logMessages[1]);
            StringAssert.Contains("SequenceNode", _logMessages[2]);
        }

        /// <summary>
        /// Tests that logger handles nested composite nodes.
        /// </summary>
        [Test]
        public void BehaviourTreeLogger_NestedCompositeNodes_WrapsAllLevels()
        {
            // Arrange
            var settings = new LoggerSettings(_logAction);
            var logger = new BehaviourTreeLogger(settings);
            
            var innerChildren = new IBehaviourTreeNode[]
            {
                new TestActionNode(NodeState.Success),
                new TestActionNode(NodeState.Success)
            };
            var innerSequence = new SequenceNode(innerChildren);
            
            var outerChildren = new IBehaviourTreeNode[]
            {
                innerSequence,
                new TestActionNode(NodeState.Success)
            };
            var outerSelector = new SelectorNode(outerChildren);

            // Act
            var wrappedNode = logger.WrapWithLogging(outerSelector);
            wrappedNode.Tick();

            // Assert
            // Should log: innerChild1, innerChild2, innerSequence, outerSelector
            Assert.AreEqual(4, _logMessages.Count);
        }

        /// <summary>
        /// Tests that logger includes hash code in log messages.
        /// </summary>
        [Test]
        public void LoggingNodeDecorator_LogMessage_IncludesHashCode()
        {
            // Arrange
            var settings = new LoggerSettings(_logAction);
            var node = new TestActionNode(NodeState.Success);
            var decorator = new LoggingNodeDecorator(node, settings);

            // Act
            decorator.Tick();

            // Assert
            Assert.AreEqual(1, _logMessages.Count);
            StringAssert.Contains($"Node hash code: {node.GetHashCode()}", _logMessages[0]);
        }

        /// <summary>
        /// Tests that logger properly disposes wrapped nodes.
        /// </summary>
        [Test]
        public void LoggingNodeDecorator_Dispose_DisposesInnerNode()
        {
            // Arrange
            var settings = new LoggerSettings(_logAction);
            var node = new TestActionNode(NodeState.Success);
            var decorator = new LoggingNodeDecorator(node, settings);

            // Act
            decorator.Dispose();

            // Assert
            Assert.IsTrue(node.IsDisposed);
        }

        /// <summary>
        /// Tests that logger handles inverter nodes correctly.
        /// </summary>
        [Test]
        public void BehaviourTreeLogger_WrapInverterNode_WrapsChild()
        {
            // Arrange
            var settings = new LoggerSettings(_logAction);
            var logger = new BehaviourTreeLogger(settings);
            var child = new TestActionNode(NodeState.Success);
            var inverter = new InverterNode(child);

            // Act
            var wrappedNode = logger.WrapWithLogging(inverter);
            wrappedNode.Tick();

            // Assert
            // Should log: child (Success), inverter (Failure due to inversion)
            Assert.AreEqual(2, _logMessages.Count);
            StringAssert.Contains("TestActionNode", _logMessages[0]);
            StringAssert.Contains("Success", _logMessages[0]);
            StringAssert.Contains("InverterNode", _logMessages[1]);
            StringAssert.Contains("Failure", _logMessages[1]);
        }

        #region Test Node Implementations

        /// <summary>
        /// Simple test node that returns a fixed state.
        /// </summary>
        private class TestActionNode : IBehaviourTreeNode
        {
            private readonly NodeState _state;
            public bool IsDisposed { get; private set; }

            public TestActionNode(NodeState state)
            {
                _state = state;
            }

            public NodeState Tick()
            {
                return _state;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        /// <summary>
        /// Test node that returns different states on sequential ticks.
        /// </summary>
        private class TestSequentialNode : IBehaviourTreeNode
        {
            private int _tickCount = 0;
            private readonly NodeState[] _states = { NodeState.Running, NodeState.Success, NodeState.Failure };

            public NodeState Tick()
            {
                var state = _states[_tickCount % _states.Length];
                _tickCount++;
                return state;
            }

            public void Dispose() { }
        }

        #endregion
    }
}


