namespace BehaviourTree.Source.Logger
{
/// <summary>
/// Decorator that adds logging functionality to behaviour tree nodes.
/// Logs state changes with configurable prefix/postfix and includes node hash codes.
/// </summary>
public class LoggingNodeDecorator : IBehaviourTreeNode
{
	private readonly IBehaviourTreeNode _inner;
	private readonly LoggerSettings _loggerSettings;
	private NodeState _previousState;

	/// <summary>
	/// Initializes a new instance of the LoggingNodeDecorator class.
	/// </summary>
	/// <param name="inner">The node to decorate with logging.</param>
	/// <param name="loggerSettings">The logger configuration settings.</param>
	public LoggingNodeDecorator(IBehaviourTreeNode inner, LoggerSettings loggerSettings)
	{
		_inner = inner;
		_loggerSettings = loggerSettings;
	}

	/// <summary>
	/// Executes the inner node and logs state changes.
	/// </summary>
	/// <returns>The state returned by the inner node.</returns>
	public NodeState Tick()
	{
		var state = _inner.Tick();

		if (_previousState == state)
		{
			return state;
		}

		_previousState = state;
		var logPostfix = _loggerSettings.Postfix;
		var logPrefix = _loggerSettings.Prefix;
		var logAction = _loggerSettings.LogAction;
		var nodeName = _inner.GetType().Name;

		logAction.Invoke($"{logPrefix} {nodeName} → {state} {logPostfix}" + $"Node hash code: {_inner.GetHashCode()}");

		return state;
	}

	/// <summary>
	/// Disposes the inner node.
	/// </summary>
	public void Dispose()
	{
		_inner.Dispose();
	}
}
}