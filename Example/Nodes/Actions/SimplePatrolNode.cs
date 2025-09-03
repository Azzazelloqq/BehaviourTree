using BehaviourTree.Source;
using UnityEngine;

namespace BehaviourTree.Example.Nodes.Actions
{
/// <summary>
/// Simple patrol action node for basic examples that don't require full movement agent.
/// </summary>
public class SimplePatrolNode : IBehaviourTreeNode
{
	private float _patrolTime;
	private const float PatrolDuration = 3f;

	/// <inheritdoc/>
	public NodeState Tick()
	{
		_patrolTime += Time.deltaTime;
		
		if (_patrolTime < PatrolDuration)
		{
			Debug.Log($"Patrolling... {_patrolTime:F1}/{PatrolDuration}s");
			return NodeState.Running;
		}
		
		Debug.Log("Patrol point reached!");
		_patrolTime = 0f;
		return NodeState.Success;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// No cleanup needed
	}
}
}
