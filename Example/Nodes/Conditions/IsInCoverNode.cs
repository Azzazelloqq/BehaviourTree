using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Conditions
{
/// <summary>
/// Condition node that checks if the agent is currently in cover.
/// </summary>
public class IsInCoverNode : IBehaviourTreeNode
{
	private readonly IMovementAgent _agent;

	/// <summary>
	/// Initializes a new instance of the IsInCoverNode class.
	/// </summary>
	/// <param name="agent">The movement agent to check.</param>
	public IsInCoverNode(IMovementAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		return _agent.IsInCover 
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
