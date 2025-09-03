# Behaviour Tree Module

A flexible and extensible behaviour tree implementation for Unity game AI and decision-making systems.

## Overview

This module provides a complete behaviour tree framework that allows you to create complex AI behaviors by composing simple, reusable nodes. The system supports standard behaviour tree patterns including sequences, selectors, decorators, and custom action/condition nodes.

## Features

- **Core Node Types**: Selector, Sequence, and Inverter nodes
- **Extensible Architecture**: Easy to create custom nodes by implementing `IBehaviourTreeNode`
- **Built-in Logging**: Comprehensive logging system for debugging tree execution
- **Memory Efficient**: Proper disposal patterns to prevent memory leaks
- **Unity Integration**: Designed specifically for Unity projects

## Architecture

### Core Interfaces

- **`IBehaviourTree`**: Main interface for behaviour trees that can be ticked and disposed
- **`IBehaviourTreeNode`**: Base interface for all nodes in the tree
- **`ICompositeNode`**: Interface for nodes that contain child nodes
- **`IReadOnlyBehaviourTreeNode`**: Marker interface for read-only node access

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

### Simple AI Behaviour

```csharp
// Create a simple AI that checks health and acts accordingly
var rootNode = new SelectorNode(new IBehaviourTreeNode[]
{
    // Priority 1: Heal if health is low
    new SequenceNode(new IBehaviourTreeNode[]
    {
        new HealthCheckNode(20),  // Check if health < 20
        new HealActionNode()      // Perform healing
    }),
    
    // Priority 2: Attack if enemy in range
    new SequenceNode(new IBehaviourTreeNode[]
    {
        new EnemyInRangeNode(10f),
        new AttackActionNode()
    }),
    
    // Default: Patrol
    new PatrolActionNode()
});

// Execute the tree
rootNode.Tick();
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

### Custom Node Implementation

```csharp
public class CustomActionNode : IBehaviourTreeNode
{
    private float _progress = 0f;
    
    public NodeState Tick()
    {
        _progress += Time.deltaTime;
        
        if (_progress < 2f)
            return NodeState.Running;
        
        // Action completed
        _progress = 0f;
        return NodeState.Success;
    }
    
    public void Dispose()
    {
        // Cleanup resources if needed
    }
}
```

## Testing

The module includes comprehensive unit tests and integration tests:

- **NodeTests.cs**: Unit tests for individual node types
- **LoggerTests.cs**: Tests for the logging system
- **IntegrationTests.cs**: Complex scenario tests

Run tests using Unity Test Runner in the Editor.

## Best Practices

1. **Node Granularity**: Keep individual nodes simple and focused on a single responsibility
2. **State Management**: Use Running state for actions that take multiple frames
3. **Resource Cleanup**: Always implement Dispose() to prevent memory leaks
4. **Debugging**: Use the built-in logger during development to understand tree execution
5. **Tree Structure**: Design trees with clear priority ordering (most important behaviors first in selectors)

## Performance Considerations

- Nodes are evaluated synchronously during each Tick()
- Avoid heavy computations in Tick() methods
- Use Running state for long-running operations
- Consider caching expensive checks between ticks

## Integration with Unity

The behaviour tree can be integrated with Unity MonoBehaviours:

```csharp
public class AIController : MonoBehaviour
{
    private IBehaviourTreeNode _rootNode;
    
    void Start()
    {
        // Initialize your behaviour tree
        _rootNode = BuildBehaviourTree();
    }
    
    void Update()
    {
        // Tick the tree every frame
        _rootNode?.Tick();
    }
    
    void OnDestroy()
    {
        // Clean up resources
        _rootNode?.Dispose();
    }
}
```

## License

This module is part of the Unity Modules project.

