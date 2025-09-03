using UnityEngine;
using BehaviourTree.Source;

namespace BehaviourTree.Example.Agents.Base
{
/// <summary>
/// Agent interface for movement-related functionality.
/// </summary>
public interface IMovementAgent : IBehaviourTreeAgent
{
	/// <summary>
	/// Gets a value indicating whether the agent is currently in cover.
	/// </summary>
	bool IsInCover { get; }
	
	/// <summary>
	/// Gets the last known position of the target.
	/// </summary>
	Vector3 LastKnownTargetPosition { get; }
	
	/// <summary>
	/// Gets a value indicating whether the agent is currently moving.
	/// </summary>
	bool IsMoving { get; }
	
	/// <summary>
	/// Finds available cover positions.
	/// </summary>
	/// <returns>True if cover was found, false otherwise.</returns>
	bool FindCover();
	
	/// <summary>
	/// Moves to the nearest cover position.
	/// </summary>
	void MoveToCover();
	
	/// <summary>
	/// Pursues the current target.
	/// </summary>
	void PursueTarget();
	
	/// <summary>
	/// Moves to the last known target position.
	/// </summary>
	void MoveToLastKnownPosition();
	
	/// <summary>
	/// Performs a random search pattern.
	/// </summary>
	void PerformRandomSearch();
	
	/// <summary>
	/// Patrols the area.
	/// </summary>
	void Patrol();
	
	/// <summary>
	/// Scans the surrounding area.
	/// </summary>
	void ScanArea();
}
}
