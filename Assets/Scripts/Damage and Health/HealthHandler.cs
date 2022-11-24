using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HealthHandler : MonoBehaviour
{
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth;

    [Header("Damage Resistance")]
    [SerializeField] float allDamageReduction;
    [SerializeField] float physicalDamageReduction;
    [SerializeField] float holyDamageReduction;

    [Header("CoolDown")]
    [SerializeField] bool hasCooldown;
    [SerializeField] float coolDownDuration;

    [Header("Events")]
    public UnityEvent<float> OnHealthChange;
    public UnityEvent<DamageInfo> OnHit;
    public UnityEvent OnReceiveNormalDamage;
    public UnityEvent<DamageInfo> OnReceiveCriticalDamage;
    public UnityEvent OnHitWhileInvulnerable;
    public UnityEvent OnStun;
    public UnityEvent OnHeal;
    public UnityEvent<DamageInfo> OnDeath;

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
        OnHealthChange?.Invoke(GetNormalisedHealth());
        if (hasCooldown) coolDownTime = new WaitForSeconds(coolDownDuration);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChange?.Invoke(GetNormalisedHealth());
    }

    public void ModifyAllDamageResistance(float allDamageResistance, bool increase = true)
    {
        if (increase) this.allDamageReduction += allDamageResistance; else this.allDamageReduction -= allDamageResistance;
    }

    public void ModifyPhysicalDamageResistance(float physicalDamageResistance, bool increase = true)
    {
        if (increase) this.physicalDamageReduction += physicalDamageResistance; else this.physicalDamageReduction -= physicalDamageResistance;
    }

    public void ModifyHolyDamageResistance(float holyDamageResistance, bool increase = true)
    {
        if (increase) this.holyDamageReduction += holyDamageResistance; else this.holyDamageReduction -= holyDamageResistance;
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

            float damageAfterResisDeduction = Utility.CalculateValueWithPercentage(damageInfo.damageAmount, allDamageReduction, false);

            if (damageInfo.damageType == DamageInfo.DamageType.Physical)
            {
                damageAfterResisDeduction = Utility.CalculateValueWithPercentage(damageInfo.damageAmount, physicalDamageReduction, false);
            }
            else if (damageInfo.damageType == DamageInfo.DamageType.Holy)
            {
                damageAfterResisDeduction = Utility.CalculateValueWithPercentage(damageInfo.damageAmount, holyDamageReduction, false);
            }
            
            damageInfo.damageAmount = damageAfterResisDeduction;
            finalDamage = damageAfterResisDeduction;

            currentHealth -= finalDamage;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnDeath.Invoke(damageInfo);
            }
            else
            {
                TriggerHealthEvents();
                if (damageInfo.stun) OnStun.Invoke();;
            }
            OnHealthChange?.Invoke(GetNormalisedHealth());

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
        OnHealthChange?.Invoke(GetNormalisedHealth());

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