using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Actions
{
/// <summary>
/// Action node that finds available cover positions.
/// </summary>
public class FindCoverNode : IBehaviourTreeNode
{
	private readonly IMovementAgent _agent;

	/// <summary>
	/// Initializes a new instance of the FindCoverNode class.
	/// </summary>
	/// <param name="agent">The movement agent that will find cover.</param>
	public FindCoverNode(IMovementAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		return _agent.FindCover() 
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
