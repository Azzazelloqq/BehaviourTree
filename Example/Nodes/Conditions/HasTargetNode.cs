using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Conditions
{
/// <summary>
/// Condition node that checks if the agent has a target.
/// </summary>
public class HasTargetNode : IBehaviourTreeNode
{
	private readonly ICombatAgent _agent;

	/// <summary>
	/// Initializes a new instance of the HasTargetNode class.
	/// </summary>
	/// <param name="agent">The combat agent to check.</param>
	public HasTargetNode(ICombatAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		return _agent.HasTarget 
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
