using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackfathomAbilityController : GameUnitAbilityController
{
    [SerializeField] private GameObject enGarde;
    
    Coroutine vengefulStanceRoutine;

    protected override void AssignSetAbilityActions(AbilitySet abilitySet)
    {
        foreach (Ability ability in abilitySet.abilities)
        {
            switch (ability.abilitySO.GetAbilityType<BlackfathomAbilities>())
            {
                case BlackfathomAbilities.BlackfathomHamstring: ability.SetAbilityAction(BlackfathomHamstring); break;
                case BlackfathomAbilities.Bash: ability.SetAbilityAction(Bash); break;
                case BlackfathomAbilities.VengefulStance: ability.SetAbilityAction(VengefulStance); break;
                case BlackfathomAbilities.MyrmidonSlash: ability.SetAbilityAction(MyrmidonSlash); break;
                default: break;
            }
        }
    }

    public void BlackfathomHamstring(Ability ability)
    {
        float damage = Utility.CalculateValueWithPercentage(gameUnit.GetStat().GetCharacterClassSO().minbaseDamage, ability.abilitySO.abilityAttributie.GetAbilityValueByID("DamageInc").GetValue<float>(), true);

        DamageInfo damageInfo = new DamageInfo(gameUnit);
        damageInfo.SetUp(DamageInfo.DamageType.Physical, damage, false);
        gameUnit.GetTarget().GetComponent<HealthHandler>().TakeDamage(damageInfo);

        buffManager.SendBuff(ability.abilitySO.buff, gameUnit.GetTarget());
    }
    
    public void Bash(Ability ability)
    {
        buffManager.SendBuff(ability.abilitySO.buff, gameUnit.GetTarget());
        Debug.Log("Bash");
    }

    public void VengefulStance(Ability ability)
    {
        float duration = ability.abilitySO.abilityAttributie.GetAbilityValueByID("Duration").GetValue<float>();

        if (vengefulStanceRoutine != null)
        {
            Debug.Log("Reset VengefulStance");
            StopCoroutine(vengefulStanceRoutine);
            gameUnit.HealthHandler.OnHit.RemoveListener(OnHit);
            gameUnit.Damager.ModifyDamageReduction(ability.abilitySO.abilityAttributie.GetAbilityValueByID("DamageReduction").GetValue<float>(), false);
        }
        
        void OnHit(DamageInfo damageInfo)
        {
            DamageInfo newDamageInfo = new DamageInfo(gameUnit);
            newDamageInfo.damageType = DamageInfo.DamageType.Physical;
            newDamageInfo.damageAmount = Utility.CalculatePercentageOfValue(damageInfo.damageAmount, ability.abilitySO.abilityAttributie.GetAbilityValueByID("Damage Return").GetValue<float>());
            newDamageInfo.reflect = true;
            if (damageInfo.owner) damageInfo.owner.HealthHandler.TakeDamage(newDamageInfo);
        }

        vengefulStanceRoutine = StartCoroutine(Utility.TimedAbility         
        (() =>
        {
            Debug.Log("Start VengefulStance");

            PopUpTextManager.Instance.PopUpText(transform, ability.abilitySO.abilityAttributie.GetAbilityValueByID("Visual").GetValue<string>(), Color.red);
            
            gameUnit.CanMove(false);
            gameUnit.Damager.ModifyDamageReduction(ability.abilitySO.abilityAttributie.GetAbilityValueByID("DamageReduction").GetValue<float>());

            gameUnit.HealthHandler.OnHit.AddListener(OnHit);

            Debug.Log("Count: " +gameUnit.HealthHandler.OnHit.GetPersistentEventCount());
        }
        , duration, () =>
        {
            Debug.Log("End VengefulStance");

            gameUnit.CanMove(true);
            gameUnit.Damager.ModifyDamageReduction(ability.abilitySO.abilityAttributie.GetAbilityValueByID("DamageReduction").GetValue<float>(), false);
            gameUnit.HealthHandler.OnHit.RemoveListener(OnHit);
            vengefulStanceRoutine = null;
        }));
    }
    
    public void MyrmidonSlash(Ability ability)
    {
        Debug.Log("MyrmidonSlash");
        PopUpTextManager.Instance.PopUpText(transform, ability.abilitySO.abilityAttributie.GetAbilityValueByID("Visual").GetValue<string>(), Color.red);
        gameUnit.Damager.CanDoubleDamage();
    }
}
