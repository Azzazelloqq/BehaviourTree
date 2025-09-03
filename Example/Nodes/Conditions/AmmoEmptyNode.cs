using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Conditions
{
/// <summary>
/// Condition node that checks if the agent is out of ammunition.
/// </summary>
public class AmmoEmptyNode : IBehaviourTreeNode
{
	private readonly ICombatAgent _agent;

	/// <summary>
	/// Initializes a new instance of the AmmoEmptyNode class.
	/// </summary>
	/// <param name="agent">The combat agent to check.</param>
	public AmmoEmptyNode(ICombatAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		return _agent.Ammo <= 0 
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
