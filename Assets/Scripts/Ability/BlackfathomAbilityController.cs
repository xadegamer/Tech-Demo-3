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
        damageInfo.SetUp(DamageInfo.DamageType.Physical, damage, false, false);
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
            gameUnit.HealthHandler.OnHit.RemoveAllListeners();
            gameUnit.Damager.ModifyDamageReduction(ability.abilitySO.abilityAttributie.GetAbilityValueByID("DamageReduction").GetValue<float>(), false);
        }
        
        vengefulStanceRoutine = StartCoroutine(Utility.TimedAbility
            
        (ability, () =>
        {
            Debug.Log("Start VengefulStance");

            PopUpTextManager.Instance.PopUpText(transform, ability.abilitySO.abilityAttributie.GetAbilityValueByID("Visual").GetValue<string>(), Color.red);
            
            gameUnit.CanMove(false);
            gameUnit.Damager.ModifyDamageReduction(ability.abilitySO.abilityAttributie.GetAbilityValueByID("DamageReduction").GetValue<float>());

            DamageInfo damageInfo = new DamageInfo(gameUnit);
            damageInfo.owner = gameUnit;
            damageInfo.damageType = DamageInfo.DamageType.Physical;

            gameUnit.HealthHandler.OnHit.AddListener(damageInfo =>
            {
                damageInfo.damageAmount = Utility.CalculatePercentageOfValue(damageInfo.damageAmount, ability.abilitySO.abilityAttributie.GetAbilityValueByID("Damage Return").GetValue<float>());
                if(damageInfo.owner)damageInfo.owner.HealthHandler.TakeDamage(damageInfo);
            });;
        }
        
        , duration, (ability) =>
        {
            Debug.Log("End VengefulStance");

            gameUnit.CanMove(true);
            gameUnit.Damager.ModifyDamageReduction(ability.abilitySO.abilityAttributie.GetAbilityValueByID("DamageReduction").GetValue<float>(), false);
            gameUnit.HealthHandler.OnHit.RemoveAllListeners();
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
