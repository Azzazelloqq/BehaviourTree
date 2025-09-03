using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Nodes.Actions
{
/// <summary>
/// Action node that moves the agent to cover.
/// </summary>
public class MoveToCoverNode : IBehaviourTreeNode
{
	private readonly IMovementAgent _agent;
	private bool _movementStarted;

	/// <summary>
	/// Initializes a new instance of the MoveToCoverNode class.
	/// </summary>
	/// <param name="agent">The movement agent that will move to cover.</param>
	public MoveToCoverNode(IMovementAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		// If already in cover, succeed
		if (_agent.IsInCover)
		{
			_movementStarted = false;
			return NodeState.Success;
		}
		
		// Start movement if not started
		if (!_movementStarted)
		{
			_agent.MoveToCover();
			_movementStarted = true;
		}
		
		// Continue moving until in cover
		return _agent.IsMoving || !_agent.IsInCover
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
