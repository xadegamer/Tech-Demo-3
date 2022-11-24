using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Damager : MonoBehaviour
{
    [SerializeField] DamageInfo damageInfo;
 
    [Tooltip("Insert taget object layer")]
    [SerializeField] protected LayerMask targetLayer;

    [Header("Damage")]
    [SerializeField] protected float chanceToHit;
    [SerializeField] protected float minDamage;
    [SerializeField] protected float maxDamage;
    
    [SerializeField] protected float chanceToCrit;
    [SerializeField] protected float criticalDamageMultiplier;

    [Header("Damage")]
    [SerializeField] protected float damageDelay;

    [Header("Damage Reduction")]
    [SerializeField] protected float damageReductionPer;

    public Action<HealthHandler> OnHitTargetHealth;

    public Action<float> OnCriticalHit;

    public Action<float> OnHit;

    [SerializeField] protected bool destroyOnImpact;

    [SerializeField] protected bool hasHit;

    private bool doubleDamage = false;

    private bool stun = false;

    public void SetUp(GameUnit owner, float minDamage, float maxDamage, float chanceToHit, float chanceToCrit, float criticalDamageMultiplier)
    {
        this.damageInfo.owner = owner;
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
        this.chanceToHit = chanceToHit;
        this.chanceToCrit = chanceToCrit;
        this.criticalDamageMultiplier = criticalDamageMultiplier;
    }
    
    public void DealDamage(Collider2D collision)
    {
        if (Utility.CalculateChance(chanceToHit))
        {
            float damage = Utility.CalculateValueWithPercentage(UnityEngine.Random.Range(minDamage, maxDamage + 1), damageReductionPer, false);
            float criticalDmgMultiplier = Utility.CalculateChance(chanceToCrit) ? criticalDamageMultiplier : 0;

            damageInfo.critical = criticalDmgMultiplier > 0;

            damageInfo.damageAmount = damageInfo.critical  ? damage * criticalDamageMultiplier : damage;

            if (doubleDamage){ damageInfo.damageAmount *= 2 ; doubleDamage = false;}
            
            if (collision.TryGetComponent(out HealthHandler healthSystem))
            {
                OnHitTargetHealth?.Invoke(healthSystem);
                float damageDealth =  healthSystem.TakeDamage(damageInfo);
                OnHit?.Invoke(damageDealth);
                if (damageInfo.critical) OnCriticalHit?.Invoke(damageDealth); 
            }
        }
        else
        {
            PopUpTextManager.Instance.PopUpText(transform, "Miss", Color.red);
        }
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

    public bool HasHit() { return hasHit; }
}

[System.Serializable]
public class DamageInfo
{
    public enum DamageType { Physical, Holy, Spell}

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

