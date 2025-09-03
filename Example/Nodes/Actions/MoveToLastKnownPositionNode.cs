using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Actions
{
/// <summary>
/// Action node that moves to the last known target position.
/// </summary>
public class MoveToLastKnownPositionNode : IBehaviourTreeNode
{
	private readonly IMovementAgent _agent;

	/// <summary>
	/// Initializes a new instance of the MoveToLastKnownPositionNode class.
	/// </summary>
	/// <param name="agent">The movement agent that will move to the last known position.</param>
	public MoveToLastKnownPositionNode(IMovementAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		_agent.MoveToLastKnownPosition();
		return NodeState.Running;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// No cleanup needed
	}
}
}
