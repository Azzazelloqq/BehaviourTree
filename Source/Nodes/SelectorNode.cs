using System.Collections.Generic;

namespace BehaviourTree.Source.Nodes
{
/// <summary>
/// A selector node that executes its children in order until one succeeds.
/// Returns Success if any child succeeds, Failure if all children fail, or Running if a child is running.
/// </summary>
public class SelectorNode : ICompositeNode
{
	/// <inheritdoc/>
	public IReadOnlyList<IReadOnlyBehaviourTreeNode> Children => _children;

	private readonly IBehaviourTreeNode[] _children;

	/// <summary>
	/// Initializes a new instance of the SelectorNode class.
	/// </summary>
	/// <param name="children">The child nodes to execute in order.</param>
	public SelectorNode(IBehaviourTreeNode[] children)
	{
		_children = children;
	}

	/// <summary>
	/// Executes children in order until one succeeds or is running.
	/// </summary>
	/// <returns>Success if any child succeeds, Running if any child is running, Failure if all fail.</returns>
	public NodeState Tick()
	{
		foreach (var child in _children)
		{
			var state = child.Tick();
			if (state != NodeState.Failure)
			{
				return state;
			}
		}

		return NodeState.Failure;
	}

	/// <summary>
	/// Disposes all child nodes.
	/// </summary>
	public void Dispose()
	{
		foreach (var child in _children)
		{
			child.Dispose();
		}
	}
}
}