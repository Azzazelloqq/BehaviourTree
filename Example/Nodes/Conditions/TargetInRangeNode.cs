using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Conditions
{
/// <summary>
/// Condition node that checks if the target is within a specified range.
/// </summary>
public class TargetInRangeNode : IBehaviourTreeNode
{
	private readonly ICombatAgent _agent;
	private readonly float _range;

	/// <summary>
	/// Initializes a new instance of the TargetInRangeNode class.
	/// </summary>
	/// <param name="agent">The combat agent to check.</param>
	/// <param name="range">The range to check against.</param>
	public TargetInRangeNode(ICombatAgent agent, float range)
	{
		_agent = agent;
		_range = range;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		return _agent.IsTargetInRange(_range) 
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
