using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Agents
{
/// <summary>
/// Composite agent interface for a tactical AI that combines health, combat, and movement capabilities.
/// </summary>
public interface ITacticalAIAgent : IHealthAgent, ICombatActionsAgent, IMovementAgent
{
}
}
