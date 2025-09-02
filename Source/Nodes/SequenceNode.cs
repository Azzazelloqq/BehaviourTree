using System.Collections.Generic;

namespace BehaviourTree.Source.Nodes
{
/// <summary>
/// A sequence node that executes its children in order until one fails.
/// Returns Success if all children succeed, Failure if any child fails, or Running if a child is running.
/// </summary>
public class SequenceNode : ICompositeNode
{
	/// <inheritdoc/>
	public IReadOnlyList<IReadOnlyBehaviourTreeNode> Children => _children;

	private readonly IBehaviourTreeNode[] _children;

	/// <summary>
	/// Initializes a new instance of the SequenceNode class.
	/// </summary>
	/// <param name="children">The child nodes to execute in order.</param>
	public SequenceNode(IBehaviourTreeNode[] children)
	{
		_children = children;
	}

	/// <summary>
	/// Executes children in order until one fails or is running.
	/// </summary>
	/// <returns>Success if all children succeed, Running if any child is running, Failure if any fail.</returns>
	public NodeState Tick()
	{
		foreach (var child in _children)
		{
			var state = child.Tick();
			if (state != NodeState.Success)
			{
				return state;
			}
		}

		return NodeState.Success;
	}

	/// <summary>
	/// Disposes all child nodes.
	/// </summary>
	public void Dispose()
	{
		foreach (var behaviourTreeNode in _children)
		{
			behaviourTreeNode.Dispose();
		}
	}
}
}