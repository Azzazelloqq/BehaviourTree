using System;
using UnityEngine;

namespace BehaviourTree.Example.Agents
{
/// <summary>
/// Implementation of a tactical AI agent that provides state management and actions for behaviour tree nodes.
/// </summary>
public class TacticalAIAgent : ITacticalAIAgent
{
	private float _health;
	private float _stamina;
	private int _ammo;
	private bool _hasTarget;
	private float _targetDistance;
	private Vector3 _lastKnownTargetPosition;
	private bool _isInCover;
	private bool _isHealing;
	private bool _isReloading;
	private bool _isAiming;
	private float _healingTime;
	private float _reloadTime;
	private float _aimTime;
	private float _moveTime;

	/// <inheritdoc/>
	public string AgentName { get; }
	
	/// <inheritdoc/>
	public float Health => _health;
	
	/// <inheritdoc/>
	public float MaxHealth { get; }
	
	/// <inheritdoc/>
	public float Stamina => _stamina;
	
	/// <inheritdoc/>
	public int Ammo => _ammo;
	
	/// <inheritdoc/>
	public int MaxAmmo { get; }
	
	/// <inheritdoc/>
	public bool HasTarget => _hasTarget;
	
	/// <inheritdoc/>
	public float TargetDistance => _targetDistance;
	
	/// <inheritdoc/>
	public bool IsInCover => _isInCover;
	
	/// <inheritdoc/>
	public Vector3 LastKnownTargetPosition => _lastKnownTargetPosition;
	
	/// <inheritdoc/>
	public bool IsHealing => _isHealing;
	
	/// <inheritdoc/>
	public bool IsReloading => _isReloading;
	
	/// <inheritdoc/>
	public bool IsAiming => _isAiming;
	
	/// <inheritdoc/>
	public bool IsMoving => _moveTime > 0;

	/// <summary>
	/// Initializes a new instance of the TacticalAIAgent class.
	/// </summary>
	/// <param name="agentName">The name of the agent.</param>
	/// <param name="maxHealth">Maximum health value.</param>
	/// <param name="maxAmmo">Maximum ammunition capacity.</param>
	public TacticalAIAgent(string agentName, float maxHealth = 100f, int maxAmmo = 30)
	{
		AgentName = agentName;
		MaxHealth = maxHealth;
		MaxAmmo = maxAmmo;
		_health = maxHealth;
		_stamina = 100f;
		_ammo = maxAmmo;
		_targetDistance = float.MaxValue;
		_lastKnownTargetPosition = Vector3.zero;
	}

	/// <inheritdoc/>
	public bool IsHealthCritical(float threshold)
	{
		return _health < threshold;
	}

	/// <inheritdoc/>
	public void Heal()
	{
		if (_isHealing) return;
		
		_isHealing = true;
		_healingTime = 0f;
		Debug.Log($"[{AgentName}] Starting healing...");
	}

	/// <inheritdoc/>
	public bool IsTargetInRange(float range)
	{
		return _hasTarget && _targetDistance <= range;
	}

	/// <inheritdoc/>
	public void Reload()
	{
		if (_isReloading) return;
		
		_isReloading = true;
		_reloadTime = 0f;
		Debug.Log($"[{AgentName}] Reloading...");
	}

	/// <inheritdoc/>
	public bool FindCover()
	{
		Debug.Log($"[{AgentName}] Finding cover...");
		// Simulate finding cover (always succeeds for example)
		return true;
	}

	/// <inheritdoc/>
	public void MoveToCover()
	{
		if (_moveTime <= 0)
		{
			_moveTime = 2f; // Takes 2 seconds to reach cover
			Debug.Log($"[{AgentName}] Moving to cover...");
		}
	}

	/// <inheritdoc/>
	public void PursueTarget()
	{
		if (!_hasTarget) return;
		
		Debug.Log($"[{AgentName}] Pursuing target at distance: {_targetDistance:F1}");
		_targetDistance -= Time.deltaTime * 5f;
	}

	/// <inheritdoc/>
	public void MoveToLastKnownPosition()
	{
		Debug.Log($"[{AgentName}] Moving to last known position...");
		// In a real implementation, this would move the agent
	}

	/// <inheritdoc/>
	public void PerformRandomSearch()
	{
		Debug.Log($"[{AgentName}] Performing random search...");
		// In a real implementation, this would move the agent randomly
	}

	/// <inheritdoc/>
	public void Patrol()
	{
		Debug.Log($"[{AgentName}] Patrolling...");
		_stamina = Mathf.Min(100f, _stamina + Time.deltaTime * 2f);
	}

	/// <inheritdoc/>
	public void ScanArea()
	{
		Debug.Log($"[{AgentName}] Scanning area...");
		// In a real implementation, this would scan for enemies
	}

	/// <inheritdoc/>
	public void PerformMeleeAttack()
	{
		Debug.Log($"[{AgentName}] Melee attack!");
		_stamina -= 10f;
	}

	/// <inheritdoc/>
	public void PerformRapidFire()
	{
		if (_ammo <= 0) return;
		
		Debug.Log($"[{AgentName}] Rapid fire! Ammo: {_ammo}");
		_ammo -= 3;
	}

	/// <inheritdoc/>
	public void AimAtTarget()
	{
		if (!_isAiming)
		{
			_isAiming = true;
			_aimTime = 0f;
			Debug.Log($"[{AgentName}] Aiming...");
		}
	}

	/// <inheritdoc/>
	public void Shoot()
	{
		if (_ammo <= 0) return;
		
		Debug.Log($"[{AgentName}] Fire! Ammo: {_ammo}");
		_ammo--;
		_isAiming = false;
	}

	/// <summary>
	/// Updates the agent's state. Should be called each frame.
	/// </summary>
	/// <param name="deltaTime">Time since last update.</param>
	public void Update(float deltaTime)
	{
		// Update healing
		if (_isHealing)
		{
			_healingTime += deltaTime;
			_health = Mathf.Min(MaxHealth, _health + deltaTime * 10f);
			
			if (_healingTime >= 3f || _health >= MaxHealth)
			{
				_isHealing = false;
				_healingTime = 0f;
				Debug.Log($"[{AgentName}] Healing complete! Health: {_health:F1}");
			}
		}
		
		// Update reloading
		if (_isReloading)
		{
			_reloadTime += deltaTime;
			
			if (_reloadTime >= 2f)
			{
				_ammo = MaxAmmo;
				_isReloading = false;
				_reloadTime = 0f;
				Debug.Log($"[{AgentName}] Reload complete! Ammo: {_ammo}");
			}
		}
		
		// Update aiming
		if (_isAiming)
		{
			_aimTime += deltaTime;
		}
		
		// Update movement
		if (_moveTime > 0)
		{
			_moveTime -= deltaTime;
			
			if (_moveTime <= 0 && !_isInCover)
			{
				_isInCover = true;
				Debug.Log($"[{AgentName}] In cover!");
			}
		}
		
		// Simulate state changes for demo
		SimulateStateChanges(deltaTime);
	}
	
	/// <summary>
	/// Simulates state changes for demonstration purposes.
	/// </summary>
	private void SimulateStateChanges(float deltaTime)
	{
		// Gradually decrease health
		_health = Mathf.Max(0, _health - deltaTime * 2f);
		
		// Regenerate stamina
		_stamina = Mathf.Min(100f, _stamina + deltaTime * 5f);
		
		// Randomly detect/lose targets
		if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
		{
			_hasTarget = !_hasTarget;
			if (_hasTarget)
			{
				_targetDistance = UnityEngine.Random.Range(3f, 30f);
				_lastKnownTargetPosition = new Vector3(
					UnityEngine.Random.Range(-10f, 10f),
					0,
					UnityEngine.Random.Range(-10f, 10f)
				);
			}
		}
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// Cleanup if needed
	}
}
}
