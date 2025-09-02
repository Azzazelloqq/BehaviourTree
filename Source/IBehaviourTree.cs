using System;

namespace BehaviourTree.Source
{
/// <summary>
/// Represents a behaviour tree that can be executed and disposed.
/// </summary>
public interface IBehaviourTree : IDisposable
{
	/// <summary>
	/// Executes one tick of the behaviour tree, updating all nodes.
	/// </summary>
	public void Tick();
}
}