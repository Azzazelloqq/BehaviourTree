using BehaviourTree.Source;
using BehaviourTree.Example.Agents;
using UnityEngine;

namespace BehaviourTree.Example.Nodes.Actions
{
/// <summary>
/// Action node that aims and shoots at target.
/// </summary>
public class AimAndShootNode : IBehaviourTreeNode
{
	private readonly ICombatActionsAgent _agent;
	private float _aimTime;
	private const float AimDuration = 0.5f;

	/// <summary>
	/// Initializes a new instance of the AimAndShootNode class.
	/// </summary>
	/// <param name="agent">The combat agent that will aim and shoot.</param>
	public AimAndShootNode(ICombatActionsAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		if (_agent.Ammo <= 0)
			return NodeState.Failure;
		
		// Start aiming if not already aiming
		if (!_agent.IsAiming)
		{
			_agent.AimAtTarget();
			_aimTime = 0f;
		}
		
		_aimTime += Time.deltaTime;
		
		// Still aiming
		if (_aimTime < AimDuration)
		{
			return NodeState.Running;
		}
		
		// Fire!
		_agent.Shoot();
		_aimTime = 0f;
		return NodeState.Success;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// No cleanup needed
	}
}
}
