using System;

namespace BehaviourTree.Source
{
public interface IBehaviourTreeAgent : IDisposable
{
	public string AgentName { get; }
}
}