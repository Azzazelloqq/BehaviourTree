using System;

namespace BehaviourTree.Source
{
/// <summary>
/// Base interface for behaviour tree agents.
/// Agents provide functionality and state management for behaviour tree nodes.
/// </summary>
public interface IBehaviourTreeAgent : IDisposable
{
	/// <summary>
	/// Gets the name of the agent for identification and logging purposes.
	/// </summary>
	public string AgentName { get; }
}
}