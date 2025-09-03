using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Actions
{
/// <summary>
/// Action node that performs a random search pattern.
/// </summary>
public class RandomSearchNode : IBehaviourTreeNode
{
	private readonly IMovementAgent _agent;

	/// <summary>
	/// Initializes a new instance of the RandomSearchNode class.
	/// </summary>
	/// <param name="agent">The movement agent that will perform random search.</param>
	public RandomSearchNode(IMovementAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		_agent.PerformRandomSearch();
		return NodeState.Running;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// No cleanup needed
	}
}
}
