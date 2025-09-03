# Behaviour Tree Module

A flexible and extensible behaviour tree implementation for Unity game AI and decision-making systems with support for agent-based architecture.

## Overview

This module provides a complete behaviour tree framework that allows you to create complex AI behaviors by composing simple, reusable nodes. The system supports standard behaviour tree patterns including sequences, selectors, decorators, and custom action/condition nodes. The framework now includes an agent-based pattern that separates AI state and behavior logic from tree structure.

## Features

- **Core Node Types**: Selector, Sequence, and Inverter nodes
- **Agent-Based Architecture**: Separate state management and actions through agents
- **Extensible Architecture**: Easy to create custom nodes by implementing `IBehaviourTreeNode`
- **Built-in Logging**: Comprehensive logging system for debugging tree execution
- **Memory Efficient**: Proper disposal patterns to prevent memory leaks
- **Unity Integration**: Designed specifically for Unity projects
- **Reusable Components**: Share behavior logic across different AI implementations

## Architecture

### Core Interfaces

- **`IBehaviourTree`**: Main interface for behaviour trees that can be ticked and disposed
- **`IBehaviourTreeNode`**: Base interface for all nodes in the tree
- **`ICompositeNode`**: Interface for nodes that contain child nodes
- **`IReadOnlyBehaviourTreeNode`**: Marker interface for read-only node access
- **`IBehaviourTreeAgent`**: Base interface for agents that provide functionality to nodes

### Agent-Based Pattern

The agent pattern separates concerns:
- **Agents**: Manage state and provide actions (health, combat, movement)
- **Nodes**: Control flow and decision logic
- **Trees**: Compose nodes into complex behaviors

This separation allows for:
- Better code reuse across different AI types
- Cleaner testing of individual components
- More maintainable and scalable AI systems

### Node States

Each node returns one of four states after execution:
- **Success**: The node completed its task successfully
- **Failure**: The node failed to complete its task
- **Running**: The node is still processing and needs more ticks
- **None**: The node is in an undefined state

### Built-in Nodes

#### Selector Node
Executes children in order until one succeeds. Returns:
- Success if any child succeeds
- Running if a child is running
- Failure if all children fail

#### Sequence Node
Executes children in order until one fails. Returns:
- Success if all children succeed
- Running if a child is running  
- Failure if any child fails

#### Inverter Node
Inverts the result of its child node:
- Success becomes Failure
- Failure becomes Success
- Running remains Running

## Usage Examples

### Simple Agent-Based AI

```csharp
// Create an agent that manages AI state
public class SimpleAIAgent : IBehaviourTreeAgent, IHealthAgent, ICombatAgent
{
    public string AgentName => "SimpleAI";
    public float Health { get; private set; } = 100f;
    public bool HasTarget { get; private set; }
    // ... other properties and methods
}

// Build behaviour tree using agent
var agent = new SimpleAIAgent();
var rootNode = new SelectorNode(new IBehaviourTreeNode[]
{
    // Priority 1: Heal if health is low
    new SequenceNode(new IBehaviourTreeNode[]
    {
        new HealthCriticalNode(agent, 20f),  // Check if health < 20
        new HealNode(agent)                   // Perform healing
    }),
    
    // Priority 2: Attack if enemy in range
    new SequenceNode(new IBehaviourTreeNode[]
    {
        new HasTargetNode(agent),
        new TargetInRangeNode(agent, 10f),
        new AttackNode(agent)
    }),
    
    // Default: Patrol
    new PatrolNode(agent)
});
```

### Complex Tactical AI

```csharp
// Use composite agent interface for complex AI
public interface ITacticalAIAgent : IHealthAgent, ICombatAgent, IMovementAgent
{
}

// Create tactical AI with emergency, combat, and search behaviors
var agent = new TacticalAIAgent("TacticalAI", maxHealth: 100f, maxAmmo: 30);

var rootNode = new SelectorNode(new IBehaviourTreeNode[]
{
    BuildEmergencyBranch(agent),  // Handle critical situations
    BuildCombatBranch(agent),      // Combat behaviors
    BuildSearchBranch(agent),      // Search and investigate
    new PatrolNode(agent)          // Default patrol
});
```

### With Logging

```csharp
// Setup logger for debugging
var loggerSettings = new LoggerSettings("[BT]", "", Debug.Log);
var logger = new BehaviourTreeLogger(loggerSettings);

// Wrap tree with logging
var loggedTree = logger.WrapWithLogging(rootNode);

// Now all node executions will be logged
loggedTree.Tick();
```

### Custom Agent Implementation

```csharp
public interface ICustomAgent : IBehaviourTreeAgent
{
    bool CanPerformSpecialAction { get; }
    void PerformSpecialAction();
}

public class CustomAgent : ICustomAgent
{
    public string AgentName => "CustomAI";
    public bool CanPerformSpecialAction => true;
    
    public void PerformSpecialAction()
    {
        Debug.Log($"[{AgentName}] Performing special action!");
    }
    
    public void Dispose() { }
}
```

### Custom Node Implementation

```csharp
public class SpecialActionNode : IBehaviourTreeNode
{
    private readonly ICustomAgent _agent;
    private float _actionTime = 0f;
    
    public SpecialActionNode(ICustomAgent agent)
    {
        _agent = agent;
    }
    
    public NodeState Tick()
    {
        if (!_agent.CanPerformSpecialAction)
            return NodeState.Failure;
        
        _actionTime += Time.deltaTime;
        
        if (_actionTime < 2f)
        {
            Debug.Log("Performing special action...");
            return NodeState.Running;
        }
        
        _agent.PerformSpecialAction();
        _actionTime = 0f;
        return NodeState.Success;
    }
    
    public void Dispose()
    {
        // Cleanup resources if needed
    }
}
```

## Agent Interfaces

The framework provides several base agent interfaces:

### IHealthAgent
Manages health-related functionality:
- Health tracking and critical state detection
- Healing actions

### ICombatAgent
Handles combat-related features:
- Target tracking and range detection
- Ammunition management
- Combat actions

### IMovementAgent
Controls movement behaviors:
- Cover system
- Target pursuit
- Patrol and search patterns

## Testing

The module includes comprehensive unit tests and integration tests:

- **NodeTests.cs**: Unit tests for individual node types
- **AgentNodeTests.cs**: Tests for agent-based nodes and behaviors
- **LoggerTests.cs**: Tests for the logging system
- **IntegrationTests.cs**: Complex scenario tests

Run tests using Unity Test Runner in the Editor.

## Best Practices

1. **Agent Design**: Keep agents focused on specific domains (health, combat, movement)
2. **Node Granularity**: Keep individual nodes simple and focused on a single responsibility
3. **State Management**: Use Running state for actions that take multiple frames
4. **Resource Cleanup**: Always implement Dispose() to prevent memory leaks
5. **Interface Composition**: Use interface composition for complex agents
6. **Debugging**: Use the built-in logger during development to understand tree execution
7. **Tree Structure**: Design trees with clear priority ordering (most important behaviors first in selectors)

## Performance Considerations

- Nodes are evaluated synchronously during each Tick()
- Avoid heavy computations in Tick() methods
- Use Running state for long-running operations
- Consider caching expensive checks between ticks
- Agents should efficiently manage state updates

## Integration with Unity

The behaviour tree can be integrated with Unity MonoBehaviours:

```csharp
public class AIController : MonoBehaviour, IBehaviourTree
{
    private IBehaviourTreeNode _rootNode;
    private ITacticalAIAgent _agent;
    private BehaviourTreeLogger _logger;
    
    void Start()
    {
        // Initialize agent
        _agent = new TacticalAIAgent(gameObject.name);
        
        // Build behaviour tree
        _rootNode = BuildBehaviourTree(_agent);
        
        // Optional: Add logging
        if (enableDebugLogging)
        {
            var loggerSettings = new LoggerSettings($"[{name}]", "", Debug.Log);
            _logger = new BehaviourTreeLogger(loggerSettings);
            _rootNode = _logger.WrapWithLogging(_rootNode);
        }
    }
    
    void Update()
    {
        // Update agent state
        _agent?.Update(Time.deltaTime);
        
        // Tick the tree
        Tick();
    }
    
    public void Tick()
    {
        _rootNode?.Tick();
    }
    
    public void Dispose()
    {
        _rootNode?.Dispose();
        _agent?.Dispose();
        _logger?.Dispose();
    }
    
    void OnDestroy()
    {
        Dispose();
    }
}
```

## Example Projects

The module includes two complete examples:

1. **SimpleBehaviourTreeExample**: Basic AI with health management and combat
2. **ComplexBehaviourTreeExample**: Advanced tactical AI with emergency behaviors, combat tactics, and search patterns

Both examples demonstrate the agent-based pattern and proper Unity integration.

## License

This module is part of the Unity Modules project.