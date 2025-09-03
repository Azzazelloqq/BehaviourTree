using BehaviourTree.Example.Agents;
using BehaviourTree.Example.Nodes.Actions;
using BehaviourTree.Example.Nodes.Conditions;
using BehaviourTree.Source;
using NUnit.Framework;

namespace BehaviourTree.Tests
{
/// <summary>
/// Unit tests for agent-based behaviour tree nodes.
/// </summary>
[TestFixture]
internal class AgentNodeTests
{
	private TacticalAIAgent _agent;

	[SetUp]
	public void Setup()
	{
		_agent = new TacticalAIAgent("TestAgent", 100f, 30);
	}

	[TearDown]
	public void TearDown()
	{
		_agent?.Dispose();
	}

	#region Condition Node Tests

	/// <summary>
	/// Tests that HealthCriticalNode returns success when health is below threshold.
	/// </summary>
	[Test]
	public void HealthCriticalNode_HealthBelowThreshold_ReturnsSuccess()
	{
		// Arrange
		var threshold = 50f;
		var node = new HealthCriticalNode(_agent, threshold);
		
		// Simulate low health
		for (int i = 0; i < 30; i++)
		{
			_agent.Update(1f); // Reduce health over time
		}

		// Act
		var result = node.Tick();

		// Assert
		Assert.AreEqual(NodeState.Success, result);
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that HealthCriticalNode returns failure when health is above threshold.
	/// </summary>
	[Test]
	public void HealthCriticalNode_HealthAboveThreshold_ReturnsFailure()
	{
		// Arrange
		var threshold = 50f;
		var node = new HealthCriticalNode(_agent, threshold);

		// Act (agent starts with full health)
		var result = node.Tick();

		// Assert
		Assert.AreEqual(NodeState.Failure, result);
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that AmmoEmptyNode returns success when out of ammo.
	/// </summary>
	[Test]
	public void AmmoEmptyNode_NoAmmo_ReturnsSuccess()
	{
		// Arrange
		var node = new AmmoEmptyNode(_agent);
		
		// Empty the ammo
		for (int i = 0; i < 30; i++)
		{
			_agent.PerformRapidFire();
		}

		// Act
		var result = node.Tick();

		// Assert
		Assert.AreEqual(NodeState.Success, result);
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that AmmoEmptyNode returns failure when has ammo.
	/// </summary>
	[Test]
	public void AmmoEmptyNode_HasAmmo_ReturnsFailure()
	{
		// Arrange
		var node = new AmmoEmptyNode(_agent);

		// Act (agent starts with full ammo)
		var result = node.Tick();

		// Assert
		Assert.AreEqual(NodeState.Failure, result);
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that IsInCoverNode returns correct state.
	/// </summary>
	[Test]
	public void IsInCoverNode_ChecksCoverState_ReturnsCorrectState()
	{
		// Arrange
		var node = new IsInCoverNode(_agent);

		// Act & Assert - Not in cover initially
		var result1 = node.Tick();
		Assert.AreEqual(NodeState.Failure, result1);

		// Move to cover
		_agent.MoveToCover();
		_agent.Update(3f); // Wait for movement to complete

		// Act & Assert - Should be in cover now
		var result2 = node.Tick();
		Assert.AreEqual(NodeState.Success, result2);
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that HasTargetNode returns correct state.
	/// </summary>
	[Test]
	public void HasTargetNode_ChecksTargetState_ReturnsCorrectState()
	{
		// Arrange
		var node = new HasTargetNode(_agent);

		// Act & Assert - No target initially
		var result1 = node.Tick();
		Assert.AreEqual(NodeState.Failure, result1);

		// Simulate target detection
		_agent.Update(0.1f); // Trigger potential random target detection
		
		// The result depends on random chance, so we just verify it returns a valid state
		var result2 = node.Tick();
		Assert.That(result2, Is.EqualTo(NodeState.Success).Or.EqualTo(NodeState.Failure));
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that TargetInRangeNode checks range correctly.
	/// </summary>
	[Test]
	public void TargetInRangeNode_ChecksRange_ReturnsCorrectState()
	{
		// Arrange
		var range = 10f;
		var node = new TargetInRangeNode(_agent, range);

		// Act & Assert - No target means not in range
		var result = node.Tick();
		Assert.AreEqual(NodeState.Failure, result);
		
		node.Dispose();
	}

	#endregion

	#region Action Node Tests

	/// <summary>
	/// Tests that HealNode performs healing action.
	/// </summary>
	[Test]
	public void HealNode_LowHealth_PerformsHealing()
	{
		// Arrange
		var node = new HealNode(_agent);
		
		// Reduce health
		for (int i = 0; i < 30; i++)
		{
			_agent.Update(1f);
		}
		var initialHealth = _agent.Health;

		// Act - Start healing
		var result1 = node.Tick();
		Assert.AreEqual(NodeState.Running, result1);

		// Update agent to process healing
		_agent.Update(1f);
		var result2 = node.Tick();
		
		// Assert - Health should have increased
		Assert.Greater(_agent.Health, initialHealth);
		Assert.That(result2, Is.EqualTo(NodeState.Running).Or.EqualTo(NodeState.Success));
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that HealNode returns success when health is full.
	/// </summary>
	[Test]
	public void HealNode_FullHealth_ReturnsSuccess()
	{
		// Arrange
		var node = new HealNode(_agent);

		// Act (agent starts with full health)
		var result = node.Tick();

		// Assert
		Assert.AreEqual(NodeState.Success, result);
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that ReloadNode performs reload action.
	/// </summary>
	[Test]
	public void ReloadNode_LowAmmo_PerformsReload()
	{
		// Arrange
		var node = new ReloadNode(_agent);
		
		// Use some ammo
		for (int i = 0; i < 5; i++)
		{
			_agent.Shoot();
		}

		// Act - Start reloading
		var result1 = node.Tick();
		Assert.AreEqual(NodeState.Running, result1);

		// Update agent to process reload
		_agent.Update(3f);
		var result2 = node.Tick();
		
		// Assert - Ammo should be full
		Assert.AreEqual(_agent.MaxAmmo, _agent.Ammo);
		Assert.AreEqual(NodeState.Success, result2);
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that MeleeAttackNode performs melee attack.
	/// </summary>
	[Test]
	public void MeleeAttackNode_Execute_PerformsAttack()
	{
		// Arrange
		var node = new MeleeAttackNode(_agent);
		var initialStamina = _agent.Stamina;

		// Act
		var result = node.Tick();

		// Assert
		Assert.AreEqual(NodeState.Success, result);
		Assert.Less(_agent.Stamina, initialStamina);
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that RapidFireNode uses ammo.
	/// </summary>
	[Test]
	public void RapidFireNode_HasAmmo_UsesAmmo()
	{
		// Arrange
		var node = new RapidFireNode(_agent);
		var initialAmmo = _agent.Ammo;

		// Act
		var result = node.Tick();

		// Assert
		Assert.AreEqual(NodeState.Success, result);
		Assert.Less(_agent.Ammo, initialAmmo);
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that RapidFireNode fails when no ammo.
	/// </summary>
	[Test]
	public void RapidFireNode_NoAmmo_ReturnsFailure()
	{
		// Arrange
		var node = new RapidFireNode(_agent);
		
		// Empty the ammo
		for (int i = 0; i < 30; i++)
		{
			_agent.PerformRapidFire();
		}

		// Act
		var result = node.Tick();

		// Assert
		Assert.AreEqual(NodeState.Failure, result);
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that FindCoverNode finds cover.
	/// </summary>
	[Test]
	public void FindCoverNode_Execute_FindsCover()
	{
		// Arrange
		var node = new FindCoverNode(_agent);

		// Act
		var result = node.Tick();

		// Assert
		Assert.AreEqual(NodeState.Success, result);
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that PatrolNode performs patrol.
	/// </summary>
	[Test]
	public void PatrolNode_Execute_ReturnsRunning()
	{
		// Arrange
		var node = new PatrolNode(_agent);

		// Act
		var result = node.Tick();

		// Assert
		Assert.AreEqual(NodeState.Running, result);
		
		node.Dispose();
	}

	/// <summary>
	/// Tests that PursueTargetNode pursues target.
	/// </summary>
	[Test]
	public void PursueTargetNode_Execute_ReturnsRunning()
	{
		// Arrange
		var node = new PursueTargetNode(_agent);

		// Act
		var result = node.Tick();

		// Assert
		Assert.AreEqual(NodeState.Running, result);
		
		node.Dispose();
	}

	#endregion

	#region Agent Tests

	/// <summary>
	/// Tests that agent is properly initialized.
	/// </summary>
	[Test]
	public void TacticalAIAgent_Initialization_ProperlyInitialized()
	{
		// Arrange & Act
		var agent = new TacticalAIAgent("TestAgent", 100f, 30);

		// Assert
		Assert.AreEqual("TestAgent", agent.AgentName);
		Assert.AreEqual(100f, agent.Health);
		Assert.AreEqual(100f, agent.MaxHealth);
		Assert.AreEqual(30, agent.Ammo);
		Assert.AreEqual(30, agent.MaxAmmo);
		Assert.IsFalse(agent.HasTarget);
		Assert.IsFalse(agent.IsInCover);
		Assert.IsFalse(agent.IsHealing);
		Assert.IsFalse(agent.IsReloading);
		
		agent.Dispose();
	}

	/// <summary>
	/// Tests that agent updates state over time.
	/// </summary>
	[Test]
	public void TacticalAIAgent_Update_UpdatesState()
	{
		// Arrange
		var agent = new TacticalAIAgent("TestAgent");
		var initialHealth = agent.Health;

		// Act
		agent.Update(1f);

		// Assert - Health should decrease over time
		Assert.Less(agent.Health, initialHealth);
		
		agent.Dispose();
	}

	/// <summary>
	/// Tests that agent heals over time.
	/// </summary>
	[Test]
	public void TacticalAIAgent_Heal_IncreasesHealth()
	{
		// Arrange
		var agent = new TacticalAIAgent("TestAgent");
		
		// Reduce health
		for (int i = 0; i < 10; i++)
		{
			agent.Update(1f);
		}
		var lowHealth = agent.Health;

		// Act - Start healing
		agent.Heal();
		agent.Update(1f);

		// Assert
		Assert.Greater(agent.Health, lowHealth);
		
		agent.Dispose();
	}

	/// <summary>
	/// Tests that agent reloads ammo.
	/// </summary>
	[Test]
	public void TacticalAIAgent_Reload_RestoresAmmo()
	{
		// Arrange
		var agent = new TacticalAIAgent("TestAgent");
		
		// Use ammo
		for (int i = 0; i < 10; i++)
		{
			agent.Shoot();
		}
		Assert.Less(agent.Ammo, agent.MaxAmmo);

		// Act - Reload
		agent.Reload();
		agent.Update(3f); // Wait for reload to complete

		// Assert
		Assert.AreEqual(agent.MaxAmmo, agent.Ammo);
		
		agent.Dispose();
	}

	#endregion
}
}
