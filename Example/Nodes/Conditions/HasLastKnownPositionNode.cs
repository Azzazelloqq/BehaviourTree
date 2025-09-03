using UnityEngine;
using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Conditions
{
/// <summary>
/// Condition node that checks if there's a last known position to investigate.
/// </summary>
public class HasLastKnownPositionNode : IBehaviourTreeNode
{
	private readonly IMovementAgent _agent;

	/// <summary>
	/// Initializes a new instance of the HasLastKnownPositionNode class.
	/// </summary>
	/// <param name="agent">The movement agent to check.</param>
	public HasLastKnownPositionNode(IMovementAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		return _agent.LastKnownTargetPosition != Vector3.zero 
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
