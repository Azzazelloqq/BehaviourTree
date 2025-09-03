using System;
using BehaviourTree.Source;
using BehaviourTree.Source.Logger;
using BehaviourTree.Source.Nodes;
using BehaviourTree.Example.Agents;
using BehaviourTree.Example.Agents.Base;
using BehaviourTree.Example.Nodes.Actions;
using BehaviourTree.Example.Nodes.Conditions;
using UnityEngine;

namespace BehaviourTree.Example
{
/// <summary>
/// Simple example implementation of a behaviour tree for an AI agent using agent-based pattern.
/// Demonstrates the use of Selector, Sequence, and custom action/condition nodes with agents.
/// </summary>
public class SimpleBehaviourTreeExample : MonoBehaviour, IBehaviourTree
{
	private IBehaviourTreeNode _rootNode;
	private SimpleAIAgent _agent;
	private BehaviourTreeLogger _logger;

	[Header("Configuration")]
	[SerializeField] private bool _enableLogging = true;
	[SerializeField] private string _agentName = "SimpleAI";

	private void Start()
	{
		InitializeBehaviourTree();
	}

	/// <summary>
	/// Initializes the behaviour tree structure with nodes using agent-based pattern.
	/// Creates a tree that represents an AI agent's decision-making process.
	/// </summary>
	private void InitializeBehaviourTree()
	{
		// Create the agent
		_agent = new SimpleAIAgent(_agentName);
		
		// Build the behaviour tree
		_rootNode = new SelectorNode(new IBehaviourTreeNode[]
		{
			// First priority: Check if health is critical and heal
			new SequenceNode(new IBehaviourTreeNode[]
			{
				new HealthCriticalNode(_agent, 20f), // Check if health is below 20
				new HealNode(_agent)                  // Perform healing
			}),
			
			// Second priority: Attack if enemy is in range
			new SequenceNode(new IBehaviourTreeNode[]
			{
				new HasTargetNode(_agent),                // Check if has target
				new TargetInRangeNode(_agent, 10f),      // Check if enemy within 10 units
				new SimpleAttackNode(_agent)              // Attack the enemy
			}),
			
			// Default: Patrol
			new SimplePatrolNode()
		});

		// Setup logger for debugging if enabled
		if (_enableLogging)
		{
			var loggerSettings = new LoggerSettings($"[{_agentName}]", "", Debug.Log);
			_logger = new BehaviourTreeLogger(loggerSettings);
			_rootNode = _logger.WrapWithLogging(_rootNode);
		}
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
		
		// Execute behaviour tree every frame
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

/// <summary>
/// Simple AI agent implementation for basic behaviour tree example.
/// </summary>
public class SimpleAIAgent : IHealthAgent, ICombatAgent
{
	private float _health;
	private float _healingTime;
	private bool _isHealing;
	private float _targetDistance;
	private bool _hasTarget;

	/// <inheritdoc/>
	public string AgentName { get; }

	/// <inheritdoc/>
	public float Health => _health;

	/// <inheritdoc/>
	public float MaxHealth => 100f;

	/// <inheritdoc/>
	public bool IsHealing => _isHealing;

	/// <inheritdoc/>
	public int Ammo => 30; // Simplified - always has ammo

	/// <inheritdoc/>
	public int MaxAmmo => 30;

	/// <inheritdoc/>
	public float Stamina => 100f; // Simplified - always has stamina

	/// <inheritdoc/>
	public bool HasTarget => _hasTarget;

	/// <inheritdoc/>
	public float TargetDistance => _targetDistance;

	/// <inheritdoc/>
	public bool IsReloading => false; // Simplified - never needs reload

	/// <summary>
	/// Initializes a new instance of the SimpleAIAgent class.
	/// </summary>
	/// <param name="agentName">The name of the agent.</param>
	public SimpleAIAgent(string agentName)
	{
		AgentName = agentName;
		_health = MaxHealth;
		_targetDistance = float.MaxValue;
	}

	/// <inheritdoc/>
	public bool IsHealthCritical(float threshold)
	{
		return _health < threshold;
	}

	/// <inheritdoc/>
	public void Heal()
	{
		if (!_isHealing)
		{
			_isHealing = true;
			_healingTime = 0f;
			Debug.Log($"[{AgentName}] Starting healing...");
		}
	}

	/// <inheritdoc/>
	public bool IsTargetInRange(float range)
	{
		return _hasTarget && _targetDistance <= range;
	}

	/// <inheritdoc/>
	public void Reload()
	{
		// Not implemented in simple example
	}

	/// <summary>
	/// Performs a simple attack action.
	/// </summary>
	public void Attack()
	{
		Debug.Log($"[{AgentName}] Attacking enemy!");
	}

	/// <summary>
	/// Updates the agent's state.
	/// </summary>
	/// <param name="deltaTime">Time since last update.</param>
	public void Update(float deltaTime)
	{
		// Simulate health decrease
		_health -= deltaTime * 5f;
		_health = Mathf.Max(0, _health);

		// Update healing
		if (_isHealing)
		{
			_healingTime += deltaTime;
			_health = Mathf.Min(MaxHealth, _health + deltaTime * 10f);
			
			if (_healingTime >= 2f || _health >= MaxHealth)
			{
				_isHealing = false;
				_healingTime = 0f;
				Debug.Log($"[{AgentName}] Healing complete! Health: {_health:F1}");
			}
		}

		// Simulate enemy detection (random for example)
		if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
		{
			_hasTarget = !_hasTarget;
			if (_hasTarget)
			{
				_targetDistance = UnityEngine.Random.Range(5f, 15f);
				Debug.Log($"[{AgentName}] Enemy detected at distance: {_targetDistance:F1}");
			}
		}
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// Cleanup if needed
	}
}

/// <summary>
/// Simple attack action node for the basic example.
/// </summary>
public class SimpleAttackNode : IBehaviourTreeNode
{
	private readonly SimpleAIAgent _agent;
	private float _attackCooldown;
	private const float AttackRate = 1f;

	/// <summary>
	/// Initializes a new instance of the SimpleAttackNode class.
	/// </summary>
	/// <param name="agent">The agent that will perform the attack.</param>
	public SimpleAttackNode(SimpleAIAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		_attackCooldown -= Time.deltaTime;
		
		if (_attackCooldown <= 0f)
		{
			_agent.Attack();
			_attackCooldown = AttackRate;
			return NodeState.Success;
		}
		
		return NodeState.Running;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// No cleanup needed
	}
}
}