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

    [SerializeField] protected UnityEvent OnHit;

    [Tooltip("Gameobject will destory after it deals damage")]
    [SerializeField] protected bool destroyOnImpact;

    [SerializeField] protected bool hasHit;


    public void SetDamage(float minDamage, float maxDamage)
    {
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
    }

    public void DealDamage(Collider2D collision)
    {
        damageInfo.damageAmount = Random.Range(minDamage, maxDamage + 1);
        if (collision.TryGetComponent(out HealthHandler healthSystem)) healthSystem.TakeDamage(damageInfo);
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
}

