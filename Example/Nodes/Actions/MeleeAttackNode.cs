using BehaviourTree.Source;
using BehaviourTree.Example.Agents;

namespace BehaviourTree.Example.Nodes.Actions
{
/// <summary>
/// Action node that performs a melee attack.
/// </summary>
public class MeleeAttackNode : IBehaviourTreeNode
{
	private readonly ICombatActionsAgent _agent;

	/// <summary>
	/// Initializes a new instance of the MeleeAttackNode class.
	/// </summary>
	/// <param name="agent">The combat agent that will perform the melee attack.</param>
	public MeleeAttackNode(ICombatActionsAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		_agent.PerformMeleeAttack();
		return NodeState.Success;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// No cleanup needed
	}
}
}
