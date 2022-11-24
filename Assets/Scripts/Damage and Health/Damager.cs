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
    
    [Header("Damage Increase")]
    [SerializeField] protected float extraDamage;


    [Header("Damage Reduction")]
    [SerializeField] protected float damageReduction;

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
            float damage = Utility.CalculateValueWithPercentage(UnityEngine.Random.Range(minDamage, maxDamage + 1), damageReduction, false);

            damage = Utility.CalculateValueWithPercentage(damage, extraDamage, true);

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
    
    public void ModifyChanceToHit(float chanceToHit, bool increase = true)
    {
        if (increase) this.chanceToHit += chanceToHit; else this.chanceToHit -= chanceToHit;
    }

    public void ModifyDamageReduction(float damageReductionPer, bool increase = false)
    {
        if (increase) this.damageReduction += damageReductionPer; else this.damageReduction -= damageReductionPer;
        if (this.damageReduction < 0) this.damageReduction = 0;
    }

    public void ModifyExtraDamage(float damageIncrease, bool increase = false)
    {
        if (increase) this.extraDamage += damageIncrease; else this.extraDamage -= damageIncrease;
        if (this.extraDamage < 0) this.extraDamage = 0;
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
    public bool reflect;

    public GameUnit owner;

    public DamageInfo(GameUnit owner) { this.owner = owner; }

    public void SetUp(DamageType damageType, float damageAmount, bool critical)
    {
        this.damageType = damageType;
        this.damageAmount = damageAmount;
        this.critical = critical;
    }
}

