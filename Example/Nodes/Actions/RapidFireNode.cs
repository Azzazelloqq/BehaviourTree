using BehaviourTree.Source;
using BehaviourTree.Example.Agents;

namespace BehaviourTree.Example.Nodes.Actions
{
/// <summary>
/// Action node that performs rapid fire attack.
/// </summary>
public class RapidFireNode : IBehaviourTreeNode
{
	private readonly ICombatActionsAgent _agent;

	/// <summary>
	/// Initializes a new instance of the RapidFireNode class.
	/// </summary>
	/// <param name="agent">The combat agent that will perform rapid fire.</param>
	public RapidFireNode(ICombatActionsAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		if (_agent.Ammo <= 0)
			return NodeState.Failure;
		
		_agent.PerformRapidFire();
		return NodeState.Success;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// No cleanup needed
	}
}
}
