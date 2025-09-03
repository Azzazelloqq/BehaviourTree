using System;
using System.Collections.Generic;
using BehaviourTree.Source;
using BehaviourTree.Source.Nodes;
using UnityEngine;

namespace BehaviourTree.Example
{
    /// <summary>
    /// Advanced example showing complex behaviour tree patterns including:
    /// - Nested composite nodes
    /// - Inverter decorators
    /// - State management
    /// - Complex decision trees
    /// </summary>
    internal class ComplexBehaviourTreeExample : MonoBehaviour
    {
        private IBehaviourTreeNode _rootNode;
        private AIState _aiState;

        /// <summary>
        /// Represents the current state of the AI agent.
        /// </summary>
        internal class AIState
        {
            public float Health { get; set; } = 100f;
            public float Stamina { get; set; } = 100f;
            public int Ammo { get; set; } = 30;
            public bool HasTarget { get; set; }
            public float TargetDistance { get; set; } = float.MaxValue;
            public Vector3 LastKnownTargetPosition { get; set; }
            public bool IsInCover { get; set; }
        }

        private void Start()
        {
            _aiState = new AIState();
            BuildComplexBehaviourTree();
        }

        /// <summary>
        /// Builds a complex behaviour tree for tactical AI behavior.
        /// </summary>
        private void BuildComplexBehaviourTree()
        {
            _rootNode = new SelectorNode(new IBehaviourTreeNode[]
            {
                // Priority 1: Emergency situations
                BuildEmergencyBranch(),
                
                // Priority 2: Combat behavior
                BuildCombatBranch(),
                
                // Priority 3: Search and investigate
                BuildSearchBranch(),
                
                // Priority 4: Default patrol
                new PatrolNode(_aiState)
            });
        }

        /// <summary>
        /// Creates the emergency behavior branch for critical situations.
        /// </summary>
        private IBehaviourTreeNode BuildEmergencyBranch()
        {
            return new SequenceNode(new IBehaviourTreeNode[]
            {
                new SelectorNode(new IBehaviourTreeNode[]
                {
                    // Check for critical health
                    new HealthCriticalNode(_aiState, 25f),
                    // Check for no ammo
                    new AmmoEmptyNode(_aiState)
                }),
                
                // Find and move to cover
                new SequenceNode(new IBehaviourTreeNode[]
                {
                    new InverterNode(new IsInCoverNode(_aiState)),
                    new FindCoverNode(_aiState),
                    new MoveToCoverNode(_aiState)
                }),
                
                // Perform emergency action
                new SelectorNode(new IBehaviourTreeNode[]
                {
                    new HealNode(_aiState),
                    new ReloadNode(_aiState)
                })
            });
        }

        /// <summary>
        /// Creates the combat behavior branch.
        /// </summary>
        private IBehaviourTreeNode BuildCombatBranch()
        {
            return new SequenceNode(new IBehaviourTreeNode[]
            {
                // Check if we have a target
                new HasTargetNode(_aiState),
                
                // Combat decision tree
                new SelectorNode(new IBehaviourTreeNode[]
                {
                    // Close range: aggressive combat
                    new SequenceNode(new IBehaviourTreeNode[]
                    {
                        new TargetInRangeNode(_aiState, 5f),
                        new SelectorNode(new IBehaviourTreeNode[]
                        {
                            new MeleeAttackNode(_aiState),
                            new RapidFireNode(_aiState)
                        })
                    }),
                    
                    // Medium range: tactical combat
                    new SequenceNode(new IBehaviourTreeNode[]
                    {
                        new TargetInRangeNode(_aiState, 20f),
                        new SelectorNode(new IBehaviourTreeNode[]
                        {
                            // Use cover if available
                            new SequenceNode(new IBehaviourTreeNode[]
                            {
                                new InverterNode(new IsInCoverNode(_aiState)),
                                new FindCoverNode(_aiState),
                                new MoveToCoverNode(_aiState),
                                new AimAndShootNode(_aiState)
                            }),
                            // Direct engagement
                            new AimAndShootNode(_aiState)
                        })
                    }),
                    
                    // Long range: pursue
                    new PursueTargetNode(_aiState)
                })
            });
        }

        /// <summary>
        /// Creates the search and investigate behavior branch.
        /// </summary>
        private IBehaviourTreeNode BuildSearchBranch()
        {
            return new SequenceNode(new IBehaviourTreeNode[]
            {
                // Check if we lost the target recently
                new LostTargetRecentlyNode(_aiState),
                
                // Search pattern
                new SelectorNode(new IBehaviourTreeNode[]
                {
                    // Investigate last known position
                    new SequenceNode(new IBehaviourTreeNode[]
                    {
                        new HasLastKnownPositionNode(_aiState),
                        new MoveToLastKnownPositionNode(_aiState),
                        new ScanAreaNode(_aiState)
                    }),
                    
                    // Random search
                    new RandomSearchNode(_aiState)
                })
            });
        }

        private void Update()
        {
            // Update AI state (simulate for example)
            UpdateAIState();
            
            // Execute behaviour tree
            _rootNode?.Tick();
        }

        private void UpdateAIState()
        {
            // Simulate state changes
            _aiState.Health = Mathf.Max(0, _aiState.Health - Time.deltaTime * 2f);
            _aiState.Stamina = Mathf.Min(100, _aiState.Stamina + Time.deltaTime * 5f);
            
            // Simulate target detection
            if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
            {
                _aiState.HasTarget = !_aiState.HasTarget;
                if (_aiState.HasTarget)
                {
                    _aiState.TargetDistance = UnityEngine.Random.Range(3f, 30f);
                }
            }
        }

        private void OnDestroy()
        {
            _rootNode?.Dispose();
        }
    }

    #region Condition Nodes

    /// <summary>
    /// Checks if health is below critical threshold.
    /// </summary>
    internal class HealthCriticalNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;
        private readonly float _threshold;

        public HealthCriticalNode(ComplexBehaviourTreeExample.AIState state, float threshold)
        {
            _state = state;
            _threshold = threshold;
        }

        public NodeState Tick()
        {
            return _state.Health < _threshold ? NodeState.Success : NodeState.Failure;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Checks if ammo is empty.
    /// </summary>
    internal class AmmoEmptyNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;

        public AmmoEmptyNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            return _state.Ammo <= 0 ? NodeState.Success : NodeState.Failure;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Checks if AI is in cover.
    /// </summary>
    internal class IsInCoverNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;

        public IsInCoverNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            return _state.IsInCover ? NodeState.Success : NodeState.Failure;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Checks if AI has a target.
    /// </summary>
    internal class HasTargetNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;

        public HasTargetNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            return _state.HasTarget ? NodeState.Success : NodeState.Failure;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Checks if target is within specified range.
    /// </summary>
    internal class TargetInRangeNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;
        private readonly float _range;

        public TargetInRangeNode(ComplexBehaviourTreeExample.AIState state, float range)
        {
            _state = state;
            _range = range;
        }

        public NodeState Tick()
        {
            return _state.TargetDistance <= _range ? NodeState.Success : NodeState.Failure;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Checks if target was lost recently.
    /// </summary>
    internal class LostTargetRecentlyNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;

        public LostTargetRecentlyNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            return !_state.HasTarget && _state.LastKnownTargetPosition != Vector3.zero 
                ? NodeState.Success 
                : NodeState.Failure;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Checks if there's a last known position to investigate.
    /// </summary>
    internal class HasLastKnownPositionNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;

        public HasLastKnownPositionNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            return _state.LastKnownTargetPosition != Vector3.zero 
                ? NodeState.Success 
                : NodeState.Failure;
        }

        public void Dispose() { }
    }

    #endregion

    #region Action Nodes

    /// <summary>
    /// Finds available cover positions.
    /// </summary>
    internal class FindCoverNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;

        public FindCoverNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            Debug.Log("Finding cover...");
            // Simulate finding cover
            return NodeState.Success;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Moves to cover position.
    /// </summary>
    internal class MoveToCoverNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;
        private float _moveTime = 0f;

        public MoveToCoverNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            _moveTime += Time.deltaTime;
            
            if (_moveTime < 2f)
            {
                Debug.Log("Moving to cover...");
                return NodeState.Running;
            }
            
            _state.IsInCover = true;
            _moveTime = 0f;
            Debug.Log("In cover!");
            return NodeState.Success;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Performs healing action.
    /// </summary>
    internal class HealNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;
        private float _healTime = 0f;

        public HealNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            if (_state.Health >= 100f)
                return NodeState.Success;
            
            _healTime += Time.deltaTime;
            _state.Health += Time.deltaTime * 10f;
            
            Debug.Log($"Healing... Health: {_state.Health:F1}");
            
            if (_healTime < 3f)
                return NodeState.Running;
            
            _healTime = 0f;
            return NodeState.Success;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Reloads weapon.
    /// </summary>
    internal class ReloadNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;
        private float _reloadTime = 0f;

        public ReloadNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            if (_state.Ammo >= 30)
                return NodeState.Success;
            
            _reloadTime += Time.deltaTime;
            
            Debug.Log("Reloading...");
            
            if (_reloadTime < 2f)
                return NodeState.Running;
            
            _state.Ammo = 30;
            _reloadTime = 0f;
            Debug.Log("Reload complete!");
            return NodeState.Success;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Performs melee attack.
    /// </summary>
    internal class MeleeAttackNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;

        public MeleeAttackNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            Debug.Log("Melee attack!");
            _state.Stamina -= 10f;
            return NodeState.Success;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Performs rapid fire attack.
    /// </summary>
    internal class RapidFireNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;

        public RapidFireNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            if (_state.Ammo <= 0)
                return NodeState.Failure;
            
            Debug.Log($"Rapid fire! Ammo: {_state.Ammo}");
            _state.Ammo -= 3;
            return NodeState.Success;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Aims and shoots at target.
    /// </summary>
    internal class AimAndShootNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;
        private float _aimTime = 0f;

        public AimAndShootNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            if (_state.Ammo <= 0)
                return NodeState.Failure;
            
            _aimTime += Time.deltaTime;
            
            if (_aimTime < 0.5f)
            {
                Debug.Log("Aiming...");
                return NodeState.Running;
            }
            
            Debug.Log($"Fire! Ammo: {_state.Ammo}");
            _state.Ammo--;
            _aimTime = 0f;
            return NodeState.Success;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Pursues the target.
    /// </summary>
    internal class PursueTargetNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;

        public PursueTargetNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            Debug.Log($"Pursuing target at distance: {_state.TargetDistance:F1}");
            _state.TargetDistance -= Time.deltaTime * 5f;
            return NodeState.Running;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Moves to last known target position.
    /// </summary>
    internal class MoveToLastKnownPositionNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;

        public MoveToLastKnownPositionNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            Debug.Log("Moving to last known position...");
            return NodeState.Running;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Scans area for targets.
    /// </summary>
    internal class ScanAreaNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;
        private float _scanTime = 0f;

        public ScanAreaNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            _scanTime += Time.deltaTime;
            
            Debug.Log("Scanning area...");
            
            if (_scanTime < 3f)
                return NodeState.Running;
            
            _scanTime = 0f;
            _state.LastKnownTargetPosition = Vector3.zero;
            return NodeState.Success;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Performs random search pattern.
    /// </summary>
    internal class RandomSearchNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;

        public RandomSearchNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            Debug.Log("Random search...");
            return NodeState.Running;
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Default patrol behavior.
    /// </summary>
    internal class PatrolNode : IBehaviourTreeNode
    {
        private readonly ComplexBehaviourTreeExample.AIState _state;

        public PatrolNode(ComplexBehaviourTreeExample.AIState state)
        {
            _state = state;
        }

        public NodeState Tick()
        {
            Debug.Log("Patrolling...");
            _state.Stamina = Mathf.Min(100f, _state.Stamina + Time.deltaTime * 2f);
            return NodeState.Running;
        }

        public void Dispose() { }
    }

    #endregion
}

