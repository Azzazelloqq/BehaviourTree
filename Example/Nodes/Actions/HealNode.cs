using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Actions
{
/// <summary>
/// Action node that performs healing on the agent.
/// </summary>
public class HealNode : IBehaviourTreeNode
{
	private readonly IHealthAgent _agent;

	/// <summary>
	/// Initializes a new instance of the HealNode class.
	/// </summary>
	/// <param name="agent">The health agent to heal.</param>
	public HealNode(IHealthAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		// If health is already full, succeed immediately
		if (_agent.Health >= _agent.MaxHealth)
			return NodeState.Success;
		
		// Start healing if not already healing
		if (!_agent.IsHealing)
		{
			_agent.Heal();
		}
		
		// Return running while healing is in progress
		return _agent.IsHealing 
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
