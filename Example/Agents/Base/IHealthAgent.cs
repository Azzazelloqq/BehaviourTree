using BehaviourTree.Source;

namespace BehaviourTree.Example.Agents.Base
{
/// <summary>
/// Agent interface for health management functionality.
/// </summary>
public interface IHealthAgent : IBehaviourTreeAgent
{
	/// <summary>
	/// Gets the current health value.
	/// </summary>
	float Health { get; }
	
	/// <summary>
	/// Gets the maximum health value.
	/// </summary>
	float MaxHealth { get; }
	
	/// <summary>
	/// Checks if health is below a critical threshold.
	/// </summary>
	/// <param name="threshold">The health threshold to check against.</param>
	/// <returns>True if health is below the threshold, false otherwise.</returns>
	bool IsHealthCritical(float threshold);
	
	/// <summary>
	/// Performs healing action.
	/// </summary>
	void Heal();
	
	/// <summary>
	/// Gets a value indicating whether healing is currently in progress.
	/// </summary>
	bool IsHealing { get; }
}
}
