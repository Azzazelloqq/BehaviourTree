using UnityEngine;
using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Conditions
{
/// <summary>
/// Condition node that checks if the agent has recently lost its target.
/// </summary>
public class LostTargetRecentlyNode : IBehaviourTreeNode
{
	private readonly ICombatAgent _combatAgent;
	private readonly IMovementAgent _movementAgent;

	/// <summary>
	/// Initializes a new instance of the LostTargetRecentlyNode class.
	/// </summary>
	/// <param name="combatAgent">The combat agent to check target status.</param>
	/// <param name="movementAgent">The movement agent to check last known position.</param>
	public LostTargetRecentlyNode(ICombatAgent combatAgent, IMovementAgent movementAgent)
	{
		_combatAgent = combatAgent;
		_movementAgent = movementAgent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		return !_combatAgent.HasTarget && _movementAgent.LastKnownTargetPosition != Vector3.zero 
			? NodeState.Success 
			: NodeState.Failure;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// No cleanup needed
	}
}
}
