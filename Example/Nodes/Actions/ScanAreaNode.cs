using BehaviourTree.Source;
using BehaviourTree.Example.Agents.Base;
using UnityEngine;

namespace BehaviourTree.Example.Nodes.Actions
{
/// <summary>
/// Action node that scans the area for targets.
/// </summary>
public class ScanAreaNode : IBehaviourTreeNode
{
	private readonly IMovementAgent _agent;
	private float _scanTime;
	private const float ScanDuration = 3f;

	/// <summary>
	/// Initializes a new instance of the ScanAreaNode class.
	/// </summary>
	/// <param name="agent">The movement agent that will scan the area.</param>
	public ScanAreaNode(IMovementAgent agent)
	{
		_agent = agent;
	}

	/// <inheritdoc/>
	public NodeState Tick()
	{
		_scanTime += Time.deltaTime;
		
		_agent.ScanArea();
		
		if (_scanTime < ScanDuration)
			return NodeState.Running;
		
		_scanTime = 0f;
		return NodeState.Success;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// No cleanup needed
	}
}
}
