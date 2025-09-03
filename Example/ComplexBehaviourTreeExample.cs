using System;
using BehaviourTree.Source;
using BehaviourTree.Source.Logger;
using BehaviourTree.Source.Nodes;
using BehaviourTree.Example.Agents;
using BehaviourTree.Example.Nodes.Actions;
using BehaviourTree.Example.Nodes.Conditions;
using UnityEngine;

namespace BehaviourTree.Example
{
/// <summary>
/// Advanced example showing complex behaviour tree patterns with agent-based architecture including:
/// - Agent-based node design
/// - Nested composite nodes
/// - Inverter decorators
/// - State management through agents
/// - Complex decision trees
/// - Logging capabilities
/// </summary>
public class ComplexBehaviourTreeExample : MonoBehaviour, IBehaviourTree
{
	private IBehaviourTreeNode _rootNode;
	private TacticalAIAgent _agent;
	private BehaviourTreeLogger _logger;
	
	[Header("Configuration")]
	[SerializeField] private bool _enableLogging = true;
	[SerializeField] private string _agentName = "TacticalAI";
	[SerializeField] private float _maxHealth = 100f;
	[SerializeField] private int _maxAmmo = 30;

	private void Start()
	{
		InitializeAgent();
		BuildComplexBehaviourTree();
	}

	/// <summary>
	/// Initializes the tactical AI agent with configured parameters.
	/// </summary>
	private void InitializeAgent()
	{
		_agent = new TacticalAIAgent(_agentName, _maxHealth, _maxAmmo);
	}

	/// <summary>
	/// Builds a complex behaviour tree for tactical AI behavior using agent-based nodes.
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
			new PatrolNode(_agent)
		});
		
		// Add logging if enabled
		if (_enableLogging)
		{
			var loggerSettings = new LoggerSettings($"[{_agentName}]", "", Debug.Log);
			_logger = new BehaviourTreeLogger(loggerSettings);
			_rootNode = _logger.WrapWithLogging(_rootNode);
		}
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
				new HealthCriticalNode(_agent, 25f),
				// Check for no ammo
				new AmmoEmptyNode(_agent)
			}),
			
			// Find and move to cover
			new SequenceNode(new IBehaviourTreeNode[]
			{
				new InverterNode(new IsInCoverNode(_agent)),
				new FindCoverNode(_agent),
				new MoveToCoverNode(_agent)
			}),
			
			// Perform emergency action
			new SelectorNode(new IBehaviourTreeNode[]
			{
				new HealNode(_agent),
				new ReloadNode(_agent)
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
			new HasTargetNode(_agent),
			
			// Combat decision tree
			new SelectorNode(new IBehaviourTreeNode[]
			{
				// Close range: aggressive combat
				new SequenceNode(new IBehaviourTreeNode[]
				{
					new TargetInRangeNode(_agent, 5f),
					new SelectorNode(new IBehaviourTreeNode[]
					{
						new MeleeAttackNode(_agent),
						new RapidFireNode(_agent)
					})
				}),
				
				// Medium range: tactical combat
				new SequenceNode(new IBehaviourTreeNode[]
				{
					new TargetInRangeNode(_agent, 20f),
					new SelectorNode(new IBehaviourTreeNode[]
					{
						// Use cover if available
						new SequenceNode(new IBehaviourTreeNode[]
						{
							new InverterNode(new IsInCoverNode(_agent)),
							new FindCoverNode(_agent),
							new MoveToCoverNode(_agent),
							new AimAndShootNode(_agent)
						}),
						// Direct engagement
						new AimAndShootNode(_agent)
					})
				}),
				
				// Long range: pursue
				new PursueTargetNode(_agent)
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
			new LostTargetRecentlyNode(_agent, _agent),
			
			// Search pattern
			new SelectorNode(new IBehaviourTreeNode[]
			{
				// Investigate last known position
				new SequenceNode(new IBehaviourTreeNode[]
				{
					new HasLastKnownPositionNode(_agent),
					new MoveToLastKnownPositionNode(_agent),
					new ScanAreaNode(_agent)
				}),
				
				// Random search
				new RandomSearchNode(_agent)
			})
		});
	}

	/// <inheritdoc/>
	public void Tick()
	{
		_rootNode?.Tick();
	}

	private void Update()
	{
		// Update agent state
		_agent?.Update(Time.deltaTime);
		
		// Execute behaviour tree
		Tick();
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		_rootNode?.Dispose();
		_agent?.Dispose();
		_logger?.Dispose();
	}

	private void OnDestroy()
	{
		Dispose();
	}
}
}