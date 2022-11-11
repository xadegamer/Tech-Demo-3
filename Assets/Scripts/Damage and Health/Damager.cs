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

    private protected DamageInfo damageInfo = new DamageInfo();
    
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

    public void SetHasKnockback(bool hasKnockback)
    {
        this.hasKnockBack = hasKnockback;
    }

    public void SetKnockbackDirection(Vector2 direction)
    {
        this.knockBackDirection = direction;
    }

    public void SetKnockbackForce (float force)
    {
        this.knockBackForce = force;
    }
}
