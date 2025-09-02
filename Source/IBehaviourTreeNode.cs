using System;

namespace BehaviourTree.Source
{
/// <summary>
/// Represents a node in a behaviour tree that can be executed and disposed.
/// </summary>
public interface IBehaviourTreeNode : IDisposable, IReadOnlyBehaviourTreeNode
{
	/// <summary>
	/// Executes the node's logic and returns its current state.
	/// </summary>
	/// <returns>The state of the node after execution (Success, Failure, Running, or None).</returns>
	public NodeState Tick();
}
}