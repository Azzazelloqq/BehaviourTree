using System.Collections.Generic;

namespace BehaviourTree.Source
{
/// <summary>
/// Represents a composite node that contains child nodes.
/// </summary>
public interface ICompositeNode : IBehaviourTreeNode
{
	/// <summary>
	/// Gets the read-only list of child nodes.
	/// </summary>
	public IReadOnlyList<IReadOnlyBehaviourTreeNode> Children { get; }
}
}