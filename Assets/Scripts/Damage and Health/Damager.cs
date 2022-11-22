using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Damager : MonoBehaviour
{
    [SerializeField] DamageInfo damageInfo;

    [Header("KnockBack")]
    [SerializeField] protected bool hasKnockBack;
    [SerializeField] protected Vector2 knockBackDirection;
    [SerializeField] protected float knockBackForce;

    
    [Tooltip("Insert taget object layer")]
    [SerializeField] protected LayerMask targetLayer;

    [Tooltip("Insert the damage amount")]
    [SerializeField] protected float minDamage;
    [SerializeField] protected float maxDamage;
    [SerializeField] protected float damageReductionPer;

    [SerializeField] protected UnityEvent OnHit;

    [Tooltip("Gameobject will destory after it deals damage")]
    [SerializeField] protected bool destroyOnImpact;

    [SerializeField] protected bool hasHit;

    private bool doubleDamage = false;

    public void SetUp(GameUnit owner, float minDamage, float maxDamage)
    {
        damageInfo.owner = owner;
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
    }

    public void DealDamage(Collider2D collision)
    {
        damageInfo.damageAmount = Utility.CalculateValueWithPercentage(Random.Range(minDamage, maxDamage + 1), damageReductionPer, false);

        if (doubleDamage)
        {
            damageInfo.damageAmount *= 2;
            doubleDamage = false;
        } 

        if (collision.TryGetComponent(out HealthHandler healthSystem)) healthSystem.TakeDamage(damageInfo);
    }

    public void CanDoubleDamage()
    {
        doubleDamage = true;
    }

    public void SetDamageReducion(float damageReductionPer)
    {
        this.damageReductionPer = damageReductionPer;
    }

    public void AddDamageReduction(float damageReductionPer)
    {
        this.damageReductionPer += damageReductionPer;
    }

    public void RemoveDamageReduction(float damageReductionPer)
    {
        this.damageReductionPer -= damageReductionPer;
    }

    protected float RandomCriticalDamage( float chance, float criticalDamage)
    {
        if ((chance / 100f) >= Random.value) return criticalDamage;
        else return 0;
    }

    public bool HasHit() { return hasHit; }
}

[System.Serializable]
public class DamageInfo
{
    public enum DamageType { Melee, Spell }

    public DamageType damageType;
    public float damageAmount;
    public bool critical;
    public bool stun;
    public GameUnit owner;

    public DamageInfo(){}

    public void SetUp(DamageType damageType, float damageAmount, bool critical, bool stun)
    {
        this.damageType = damageType;
        this.damageAmount = damageAmount;
        this.critical = critical;
        this.stun = stun;
    }
}

