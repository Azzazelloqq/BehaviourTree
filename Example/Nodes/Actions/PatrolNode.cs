using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Actions
{
/// <summary>
/// Action node that performs patrol behavior.
/// Requires an agent that implements IMovementAgent interface.
/// </summary>
public class PatrolNode : IBehaviourTreeNode
{
	private readonly IMovementAgent _agent;

	/// <summary>
	/// Initializes a new instance of the PatrolNode class.
	/// </summary>
	/// <param name="agent">The movement agent that will patrol.</param>
	public PatrolNode(IMovementAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		_agent.Patrol();
		return NodeState.Running;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// No cleanup needed
	}
}
}
