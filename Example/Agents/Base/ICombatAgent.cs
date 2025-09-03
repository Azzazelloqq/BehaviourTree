using BehaviourTree.Source;

namespace BehaviourTree.Example.Agents.Base
{
/// <summary>
/// Agent interface for combat-related functionality.
/// </summary>
public interface ICombatAgent : IBehaviourTreeAgent
{
	/// <summary>
	/// Gets the current ammunition count.
	/// </summary>
	int Ammo { get; }
	
	/// <summary>
	/// Gets the maximum ammunition capacity.
	/// </summary>
	int MaxAmmo { get; }
	
	/// <summary>
	/// Gets the current stamina value.
	/// </summary>
	float Stamina { get; }
	
	/// <summary>
	/// Gets a value indicating whether the agent has a target.
	/// </summary>
	bool HasTarget { get; }
	
	/// <summary>
	/// Gets the distance to the current target.
	/// </summary>
	float TargetDistance { get; }
	
	/// <summary>
	/// Checks if the target is within the specified range.
	/// </summary>
	/// <param name="range">The range to check.</param>
	/// <returns>True if the target is within range, false otherwise.</returns>
	bool IsTargetInRange(float range);
	
	/// <summary>
	/// Performs a reload action.
	/// </summary>
	void Reload();
	
	/// <summary>
	/// Gets a value indicating whether reloading is currently in progress.
	/// </summary>
	bool IsReloading { get; }
}
}