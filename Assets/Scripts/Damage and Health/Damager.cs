using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Damager : MonoBehaviour
{
    public enum DealType { Damage}

    [SerializeField] protected DealType dealType;

    [Header("KnockBack")]
    [SerializeField] protected bool hasKnockBack;
    [SerializeField] protected Vector2 knockBackDirection;
    [SerializeField] protected float knockBackForce;


    [Tooltip("Insert taget object layer")]
    [SerializeField] protected LayerMask targetLayer;

    [Tooltip("Insert the damage amount")]
    [SerializeField] protected float damage;

    [SerializeField] protected UnityEvent OnHit;

    [Tooltip("Gameobject will destory after it deals damage")]
    [SerializeField] protected bool destroyOnImpact;

    [SerializeField] DamageInfo damageInfo = new DamageInfo();

    [SerializeField] protected bool hasHit;

    public void DealDamage(Collider2D collision)
    {
        if (collision.TryGetComponent(out HealthHandler healthSystem)) healthSystem.TakeDamage(damageInfo);
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void SetDealType(DealType dealType)
    {
        this.dealType = dealType;
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
    public enum DamageType { Melee, Projectile }

    public DamageType damageType;
    public int damageAmount;
    public bool critical;
    public bool stun;

    public int damageIncrease;
    public int damageIncreasePercent;
}

