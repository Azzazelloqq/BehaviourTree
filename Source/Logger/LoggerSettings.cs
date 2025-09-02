using System;

namespace BehaviourTree.Source.Logger
{
/// <summary>
/// Configuration settings for behaviour tree logging.
/// </summary>
public readonly struct LoggerSettings
{
	/// <summary>
	/// Gets the prefix text added to all log messages.
	/// </summary>
	public string Prefix { get; }
	
	/// <summary>
	/// Gets the postfix text added to all log messages.
	/// </summary>
	public string Postfix { get; }
	
	/// <summary>
	/// Gets the action used to output log messages.
	/// </summary>
	public Action<string> LogAction { get; }

	/// <summary>
	/// Initializes a new instance of the LoggerSettings struct.
	/// </summary>
	/// <param name="prefix">The prefix text for log messages.</param>
	/// <param name="postfix">The postfix text for log messages.</param>
	/// <param name="logAction">The action to output log messages.</param>
	public LoggerSettings(string prefix, string postfix, Action<string> logAction)
	{
		Prefix = prefix;
		Postfix = postfix;
		LogAction = logAction;
	}

	/// <summary>
	/// Initializes a new instance of the LoggerSettings struct with only prefix.
	/// </summary>
	/// <param name="prefix">The prefix text for log messages.</param>
	/// <param name="logAction">The action to output log messages.</param>
	public LoggerSettings(string prefix, Action<string> logAction)
	{
		Prefix = prefix;
		Postfix = string.Empty;
		;
		LogAction = logAction;
	}

	/// <summary>
	/// Initializes a new instance of the LoggerSettings struct with only log action.
	/// </summary>
	/// <param name="logAction">The action to output log messages.</param>
	public LoggerSettings(Action<string> logAction)
	{
		LogAction = logAction;
		Prefix = string.Empty;
		Postfix = string.Empty;
	}
}
}