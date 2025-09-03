using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Conditions
{
/// <summary>
/// Condition node that checks if the agent's health is below a critical threshold.
/// </summary>
public class HealthCriticalNode : IBehaviourTreeNode
{
	private readonly IHealthAgent _agent;
	private readonly float _threshold;

	/// <summary>
	/// Initializes a new instance of the HealthCriticalNode class.
	/// </summary>
	/// <param name="agent">The health agent to check.</param>
	/// <param name="threshold">The critical health threshold.</param>
	public HealthCriticalNode(IHealthAgent agent, float threshold)
	{
		_agent = agent;
		_threshold = threshold;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		return _agent.IsHealthCritical(_threshold) 
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
