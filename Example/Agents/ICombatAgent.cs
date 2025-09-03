using BehaviourTree.Example.Agents.Base;

namespace BehaviourTree.Example.Agents
{
/// <summary>
/// Agent interface for combat actions.
/// </summary>
public interface ICombatActionsAgent : ICombatAgent
{
	/// <summary>
	/// Performs a melee attack.
	/// </summary>
	void PerformMeleeAttack();
	
	/// <summary>
	/// Performs a rapid fire attack.
	/// </summary>
	void PerformRapidFire();
	
	/// <summary>
	/// Aims at the target.
	/// </summary>
	void AimAtTarget();
	
	/// <summary>
	/// Shoots at the target.
	/// </summary>
	void Shoot();
	
	/// <summary>
	/// Gets a value indicating whether the agent is currently aiming.
	/// </summary>
	bool IsAiming { get; }
}
}
