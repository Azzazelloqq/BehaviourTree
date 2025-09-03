using System;
using BehaviourTree.Source;
using BehaviourTree.Source.Logger;
using BehaviourTree.Source.Nodes;
using UnityEngine;

namespace BehaviourTree.Example
{
    /// <summary>
    /// Example implementation of a simple behaviour tree for an AI agent.
    /// Demonstrates the use of Selector, Sequence, and custom action/condition nodes.
    /// </summary>
    public class SimpleBehaviourTreeExample : MonoBehaviour, IBehaviourTree
    {
        private IBehaviourTreeNode _rootNode;
        private BehaviourTreeLogger _logger;

        private void Start()
        {
            InitializeBehaviourTree();
        }

        /// <summary>
        /// Initializes the behaviour tree structure with nodes.
        /// Creates a tree that represents an AI agent's decision-making process.
        /// </summary>
        private void InitializeBehaviourTree()
        {
            // Setup logger for debugging
            var loggerSettings = new LoggerSettings("[BT]", "", Debug.Log);
            _logger = new BehaviourTreeLogger(loggerSettings);

            // Build the behaviour tree
            _rootNode = new SelectorNode(new IBehaviourTreeNode[]
            {
                // First priority: Check if health is critical and heal
                new SequenceNode(new IBehaviourTreeNode[]
                {
                    new HealthCheckNode(20), // Check if health is below 20
                    new HealActionNode()     // Perform healing
                }),
                
                // Second priority: Attack if enemy is in range
                new SequenceNode(new IBehaviourTreeNode[]
                {
                    new EnemyInRangeNode(10f), // Check if enemy within 10 units
                    new AttackActionNode()      // Attack the enemy
                }),
                
                // Default: Patrol
                new PatrolActionNode()
            });

            // Wrap with logging for debugging
            _rootNode = _logger.WrapWithLogging(_rootNode);
        }

        /// <summary>
        /// Executes one tick of the behaviour tree.
        /// Should be called regularly (e.g., in Update or FixedUpdate).
        /// </summary>
        public void Tick()
        {
            _rootNode?.Tick();
        }

        private void Update()
        {
            // Execute behaviour tree every frame
            Tick();
        }

        public void Dispose()
        {
            _rootNode?.Dispose();
            _logger?.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }

    /// <summary>
    /// Condition node that checks if health is below a threshold.
    /// </summary>
    public class HealthCheckNode : IBehaviourTreeNode
    {
        private readonly float _healthThreshold;
        private float _currentHealth = 100f; // Simulated health

        public HealthCheckNode(float healthThreshold)
        {
            _healthThreshold = healthThreshold;
        }

        public NodeState Tick()
        {
            // Simulate health decrease
            _currentHealth -= Time.deltaTime * 5f;
            
            if (_currentHealth < _healthThreshold)
            {
                Debug.Log($"Health critical: {_currentHealth:F1}");
                return NodeState.Success;
            }
            
            return NodeState.Failure;
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    /// <summary>
    /// Action node that performs healing.
    /// </summary>
    public class HealActionNode : IBehaviourTreeNode
    {
        private float _healingTime = 0f;
        private const float HealDuration = 2f;

        public NodeState Tick()
        {
            _healingTime += Time.deltaTime;
            
            if (_healingTime < HealDuration)
            {
                Debug.Log($"Healing... {_healingTime:F1}/{HealDuration}s");
                return NodeState.Running;
            }
            
            Debug.Log("Healing complete!");
            _healingTime = 0f;
            return NodeState.Success;
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    /// <summary>
    /// Condition node that checks if an enemy is within range.
    /// </summary>
    public class EnemyInRangeNode : IBehaviourTreeNode
    {
        private readonly float _detectionRange;

        public EnemyInRangeNode(float detectionRange)
        {
            _detectionRange = detectionRange;
        }

        public NodeState Tick()
        {
            // Simulate enemy detection
            var enemies = Physics.OverlapSphere(Vector3.zero, _detectionRange);
            
            if (enemies.Length > 0)
            {
                Debug.Log($"Enemy detected within {_detectionRange} units!");
                return NodeState.Success;
            }
            
            return NodeState.Failure;
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    /// <summary>
    /// Action node that performs an attack.
    /// </summary>
    public class AttackActionNode : IBehaviourTreeNode
    {
        private float _attackCooldown = 0f;
        private const float AttackRate = 1f;

        public NodeState Tick()
        {
            _attackCooldown -= Time.deltaTime;
            
            if (_attackCooldown <= 0f)
            {
                Debug.Log("Attacking enemy!");
                _attackCooldown = AttackRate;
                return NodeState.Success;
            }
            
            return NodeState.Running;
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    /// <summary>
    /// Action node that performs patrolling behavior.
    /// </summary>
    public class PatrolActionNode : IBehaviourTreeNode
    {
        private float _patrolTime = 0f;
        private const float PatrolDuration = 3f;

        public NodeState Tick()
        {
            _patrolTime += Time.deltaTime;
            
            if (_patrolTime < PatrolDuration)
            {
                Debug.Log($"Patrolling... {_patrolTime:F1}/{PatrolDuration}s");
                return NodeState.Running;
            }
            
            Debug.Log("Patrol point reached!");
            _patrolTime = 0f;
            return NodeState.Success;
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }
}

