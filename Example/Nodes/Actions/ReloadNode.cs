using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Actions
{
/// <summary>
/// Action node that reloads the agent's weapon.
/// </summary>
public class ReloadNode : IBehaviourTreeNode
{
	private readonly ICombatAgent _agent;

	/// <summary>
	/// Initializes a new instance of the ReloadNode class.
	/// </summary>
	/// <param name="agent">The combat agent to reload.</param>
	public ReloadNode(ICombatAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		// If ammo is already full, succeed immediately
		if (_agent.Ammo >= _agent.MaxAmmo)
			return NodeState.Success;
		
		// Start reloading if not already reloading
		if (!_agent.IsReloading)
		{
			_agent.Reload();
		}
		
		// Return running while reloading is in progress
		return _agent.IsReloading 
			? NodeState.Running 
			: NodeState.Success;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// No cleanup needed
	}
}
}
