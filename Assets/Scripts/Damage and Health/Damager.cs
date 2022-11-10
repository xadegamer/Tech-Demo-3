using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
	[Header("Damage Multiplier")]
	[SerializeField] protected bool ragedDamage;
	[SerializeField] protected float damageMultiplier = 1f;

	[Header("Damage")]
	[SerializeField] protected int damage;
	[SerializeField] protected int criticalDamage;
	[SerializeField] protected float criticalChance;
	[SerializeField] protected LayerMask targetLayer;
	[SerializeField] protected bool hasHit;

	public MonoBehaviour owner { get; private set; }

	private protected DamageInfo damageInfo =  new DamageInfo();

	public UnityEvent OnHit;

	public void SetUp(MonoBehaviour owner, int currentDamage, int currentCriticalDamage, float currentCriticalChance)
    {
		this.owner = owner;
		damage = currentDamage;
		criticalDamage = currentCriticalDamage;
		criticalChance = currentCriticalChance;
	}

	public virtual void Attack(bool knockBack){}

	public virtual void ToggleConstantAttack(bool newState) { }
	public virtual void ToggleConstantKockBackAttack(bool newState) { }

	protected int RandomCriticalDamage()
	{
		if ((criticalChance / 100f) >= Random.value) return criticalDamage;
		else return 0;
	}

	public bool HasHit() { return hasHit; }
}
