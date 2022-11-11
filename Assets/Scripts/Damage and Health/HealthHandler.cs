using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HealthHandler : MonoBehaviour
{
    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;

    [Header("Hits")]
    [SerializeField] bool useHits;
    [SerializeField] int curentHit;
    [SerializeField] int maxHit;

    [Header("Damage Resistance")]
    [SerializeField] int allDamageResistance;
    [SerializeField] int meleeDamageResistance;
    [SerializeField] int projectileDamageResistance;

    [Header("CoolDown")]
    [SerializeField] bool hasCooldown;
    [SerializeField] float coolDownDuration;

    [Header("Events")]
    public UnityEvent OnReceiveDamage;
    public UnityEvent OnReceiveNormalDamage;
    public UnityEvent OnReceiveCriticalDamage;
    public UnityEvent OnHitWhileInvulnerable;
    public UnityEvent OnHit;
    public UnityEvent OnStun;
    public UnityEvent OnHeal;
    public UnityEvent OnDied;

    [Header("HealthEvents")]
    [SerializeField] HealthEvent[] healthEvents;

    [Header("Debug")]
    [SerializeField] bool isInvulnerable = false;
    [SerializeField] bool damageDelay;

    public event EventHandler<Vector3> OnKnockback;

    WaitForSeconds coolDownTime;

    private void OnEnable()
    {
        ResetTriggerHealthEvents();

        ResetHits();
    }

    private void Start()
    {
        if (hasCooldown) coolDownTime = new WaitForSeconds(coolDownDuration);
    }

    public void SetHealth(int amount)
    {
        currentHealth = maxHealth = amount;
        if (hasCooldown) coolDownTime = new WaitForSeconds(coolDownDuration);
    }

    public void SetDamageResistance(int allDamageResist,int meleeResist, int projectileResist)
    {
        allDamageResistance = allDamageResist;
        meleeDamageResistance = meleeResist;
        projectileDamageResistance = projectileResist;
    }

    public int GetHealth() { return currentHealth; }
    public int GetMaxHealth() { return maxHealth; }


    public float GetHealthPercent() { return (float)currentHealth / (float)maxHealth; }

     public bool TakeDamage(DamageInfo damageInfo)
    {
        if (damageDelay) return false;

        if (useHits)
        {
            ManageHit();
            return false;
        }

        if (isInvulnerable) { OnHitWhileInvulnerable.Invoke(); return false;}

        if (currentHealth > 0)
        {
            if (hasCooldown) StartCoroutine(nameof(DamageDelay));

            int finalDamage = 0;
            int damageAfterResisDeduction = 0;

            damageAfterResisDeduction = CalculateDamage(damageInfo.damageAmount, allDamageResistance, false);

            if (damageInfo.damageType == DamageInfo.DamageType.Melee)
            {
                damageAfterResisDeduction = CalculateDamage(damageAfterResisDeduction, meleeDamageResistance , false);
            }
            else if (damageInfo.damageType == DamageInfo.DamageType.Projectile)
            {
                damageAfterResisDeduction = CalculateDamage(damageAfterResisDeduction, projectileDamageResistance , false);
            }

            finalDamage = CalculateDamage(damageAfterResisDeduction, damageInfo.damageIncreasePercent, true);

            finalDamage += damageInfo.damageIncrease;

            currentHealth -= finalDamage;
            
            OnReceiveDamage?.Invoke();

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnDied.Invoke();
            }
            else
            {
                TriggerHealthEvents();
                if (damageInfo.stun) OnStun.Invoke();
                else OnHit.Invoke();
            }

            if (OnKnockback != null && damageInfo.knockback) OnKnockback(this, damageInfo.direction);

            if (damageInfo.critical) OnReceiveCriticalDamage.Invoke(); OnReceiveNormalDamage.Invoke();
            PopUpTextManager.Instance.PopUpText(transform, finalDamage.ToString(), damageInfo.critical ? Color.red : Color.yellow);
        }

        return true;
    }

    public void RestoreHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        OnHeal.Invoke();

        PopUpTextManager.Instance.PopUpText(transform, amount.ToString(),  Color.green);
    }
    IEnumerator DamageDelay()
    {
        damageDelay = true;
        yield return coolDownTime;
        damageDelay = false;
    }

    int  CalculateDamage(int damage, int percentage, bool increase)
    {
        float percentageValue = ((float)percentage / 100) * damage;
        return increase ? damage += (int)percentageValue : damage -= (int)percentageValue;
    }

    public void Setvulnerability(bool newState) { isInvulnerable = newState;}


    public void TriggerHealthEvents()
    {
        float healthPercentage = useHits ? GetHitPercent() * 100f : GetHealthPercent() * 100f;

        for (int i = 0; i < healthEvents.Length; i++)
        {
            if (healthEvents[i].done) continue;

            if(healthEvents[i].healthPercentage >= healthPercentage)
            {
                healthEvents[i].OnReach?.Invoke();
                healthEvents[i].done = true;
                return;
            }
        }
    }

    public void ResetTriggerHealthEvents()
    {
        for (int i = 0; i < healthEvents.Length; i++) healthEvents[i].done = false;
    }

    public void ManageHit()
    {
        if(curentHit <= 0) return;
        curentHit--;
        TriggerHealthEvents();
        StartCoroutine(nameof(DamageDelay));

        if (curentHit <= 0)
        {
            curentHit = 0;
            OnDied.Invoke();
        }
        else OnReceiveDamage?.Invoke();
    }

    public void ResetHits()
    {
        if (useHits) curentHit = maxHit;
    }

    public float GetHitPercent() => (float)curentHit / (float)maxHit;
}

[System.Serializable]
public class HealthEvent
{
    [Tooltip("Must be between 100 and 10, Decending...")]
    [Range(100, 10)]
    public float healthPercentage;
    public UnityEvent OnReach;
    [HideInInspector]public bool done;
}