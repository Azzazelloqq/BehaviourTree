using System.Collections.Generic;

namespace BehaviourTree.Source.Nodes
{
/// <summary>
/// A decorator node that inverts the result of its child node.
/// Success becomes Failure, Failure becomes Success, Running remains Running.
/// </summary>
public class InverterNode : ICompositeNode
{
	/// <inheritdoc/>
	public IReadOnlyList<IReadOnlyBehaviourTreeNode> Children => _children;

	private readonly IBehaviourTreeNode[] _children;

	/// <summary>
	/// Initializes a new instance of the InverterNode class.
	/// </summary>
	/// <param name="child">The child node whose result will be inverted.</param>
	public InverterNode(IBehaviourTreeNode child)
	{
		_children = new[] { child };
	}

	/// <summary>
	/// Executes the child node and inverts its result.
	/// </summary>
	/// <returns>Inverted state of the child node (Success→Failure, Failure→Success, Running→Running).</returns>
	public NodeState Tick()
	{
		var state = _children[0].Tick();
		switch (state)
		{
			case NodeState.Success:
				return NodeState.Failure;
			case NodeState.Failure:
				return NodeState.Success;
			case NodeState.Running:
				return NodeState.Running;
		}

		return NodeState.Failure;
	}

	/// <summary>
	/// Disposes the child node.
	/// </summary>
	public void Dispose()
	{
		_children[0].Dispose();
	}
}
}