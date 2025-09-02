namespace BehaviourTree.Source
{
/// <summary>
/// Represents the execution state of a behaviour tree node.
/// </summary>
public enum NodeState
{
	/// <summary>
	/// The node has not been executed or is in an undefined state.
	/// </summary>
	None = 0,
	
	/// <summary>
	/// The node executed successfully and completed its task.
	/// </summary>
	Success = 1,
	
	/// <summary>
	/// The node failed to complete its task.
	/// </summary>
	Failure = 2,
	
	/// <summary>
	/// The node is still processing and needs more ticks to complete.
	/// </summary>
	Running = 3
}
}