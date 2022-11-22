using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HealthHandler : MonoBehaviour
{
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth;

    [Header("Damage Resistance")]
    [SerializeField] float allDamageResistance;
    [SerializeField] float meleeDamageResistance;
    [SerializeField] float physicalDamageReduction;

    [Header("CoolDown")]
    [SerializeField] bool hasCooldown;
    [SerializeField] float coolDownDuration;

    [Header("Events")]
    public UnityEvent OnHealthChange;
    public UnityEvent<DamageInfo> OnHit;
    public UnityEvent OnReceiveNormalDamage;
    public UnityEvent<DamageInfo> OnReceiveCriticalDamage;
    public UnityEvent OnHitWhileInvulnerable;
    public UnityEvent OnStun;
    public UnityEvent OnHeal;
    public UnityEvent OnDeath;

    [Header("HealthEvents")]
    [SerializeField] HealthEvent[] healthEvents;




    [Header("Debug")]
    [SerializeField] bool isInvulnerable = false;
    [SerializeField] bool damageDelay;

    WaitForSeconds coolDownTime;

    private void OnEnable()
    {
        ResetTriggerHealthEvents();
    }

    private void Start()
    {
        if (hasCooldown) coolDownTime = new WaitForSeconds(coolDownDuration);
    }

    public void SetHealth(float amount)
    {
        currentHealth = maxHealth = amount;
        OnHealthChange?.Invoke();
        if (hasCooldown) coolDownTime = new WaitForSeconds(coolDownDuration);
    }

    public void SetDamageResistance(float allDamageResist, float meleeResist, float physicalDamageReduction)
    {
        allDamageResistance = allDamageResist;
        meleeDamageResistance = meleeResist;
        this.physicalDamageReduction = physicalDamageReduction;
    }

    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    public float GetNormalisedHealth() { return currentHealth / maxHealth; }

    public float TakeDamage(DamageInfo damageInfo)
    {
        if (damageDelay) return 0;

        if (isInvulnerable) { OnHitWhileInvulnerable.Invoke(); return 0; }

        float finalDamage = 0;

        if (currentHealth > 0)
        {
            if (hasCooldown) StartCoroutine(nameof(DamageDelay));

            float damageAfterResisDeduction = 0;

            damageAfterResisDeduction = Utility.CalculateValueWithPercentage(damageInfo.damageAmount, allDamageResistance, false);

            if (damageInfo.damageType == DamageInfo.DamageType.Melee)
            {
                damageAfterResisDeduction = Utility.CalculateValueWithPercentage(damageAfterResisDeduction, meleeDamageResistance, false);
            }
            else if (damageInfo.damageType == DamageInfo.DamageType.Spell)
            {
                damageAfterResisDeduction = Utility.CalculateValueWithPercentage(damageAfterResisDeduction, physicalDamageReduction, false);
            }

            damageInfo.damageAmount = damageAfterResisDeduction;
            finalDamage = damageAfterResisDeduction;

            currentHealth -= finalDamage;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnDeath.Invoke();
            }
            else
            {
                TriggerHealthEvents();
                if (damageInfo.stun) OnStun.Invoke();;
            }

            OnHealthChange?.Invoke();

            if (damageInfo.critical) OnReceiveCriticalDamage.Invoke(damageInfo); OnReceiveNormalDamage.Invoke();
            PopUpTextManager.Instance.PopUpText(transform, finalDamage.ToString("F0"), damageInfo.critical ? Color.red : Color.yellow);

            OnHit?.Invoke(damageInfo);
        }
        return finalDamage;
    }

    public void RestoreHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        OnHeal.Invoke();
        OnHealthChange?.Invoke();

        PopUpTextManager.Instance.PopUpText(transform, amount.ToString("F0"),  Color.green);
    }
    
    IEnumerator DamageDelay()
    {
        damageDelay = true;
        yield return coolDownTime;
        damageDelay = false;
    }

    public void SetVulnerability(bool newState) { isInvulnerable = newState;}

    public void TriggerHealthEvents()
    {
        float healthPercentage = GetNormalisedHealth() * 100f;

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
}

[Serializable]
public class HealthEvent
{
    [Tooltip("Must be between 100 and 10, Decending...")]
    [Range(100, 10)]
    public float healthPercentage;
    public UnityEvent OnReach;
    [HideInInspector]public bool done;
}