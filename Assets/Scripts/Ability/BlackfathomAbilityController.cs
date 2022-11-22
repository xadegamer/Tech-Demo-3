using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BlackfathomAbilityController : GameUnitAbilityController
{
    [SerializeField] private GameObject enGarde;

    Coroutine vengefulStanceRoutine;

    protected override void AssignSetAbilityActions(AbilitySetSO abilitySetSO)
    {
        foreach (AbilitySO abilitySO in abilitySetSO.abilities)
        {
            switch (abilitySO.GetAbilityType<BlackfathomAbilities>())
            {
                case BlackfathomAbilities.BlackfathomHamstring: abilitySO.SetAbilityAction(BlackfathomHamstring); break;
                case BlackfathomAbilities.Bash: abilitySO.SetAbilityAction(Bash); break;
                case BlackfathomAbilities.VengefulStance: abilitySO.SetAbilityAction(VengefulStance); break;
                case BlackfathomAbilities.MyrmidonSlash: abilitySO.SetAbilityAction(MyrmidonSlash); break;
                default: break;
            }
        }
    }

    public void BlackfathomHamstring(AbilitySO abilitySO)
    {
        float damage = Utility.CalculateValueWithPercentage(gameUnit.GetStat().GetCharacterClassSO().minbaseDamage, abilitySO.abilityAttributie.GetAbilityValueByID("DamageInc").GetValue<float>(), true);
        damageInfo.SetUp(DamageInfo.DamageType.Melee, damage, false, false);
        gameUnit.GetTarget().GetComponent<HealthHandler>().TakeDamage(damageInfo);

        // 50% Movement and Attack Speed Reduction Debuff
        buffManager.SendBuff(abilitySO.buff, gameUnit.GetTarget());
        Debug.Log("BlackfathomHamstring");
    }

    public void Bash(AbilitySO abilitySO)
    {
        //Bash Debuff
        buffManager.SendBuff(abilitySO.buff, gameUnit.GetTarget());
        Debug.Log("Bash");
    }

    public void VengefulStance (AbilitySO abilitySO)
    {
        float duration = abilitySO.abilityAttributie.GetAbilityValueByID("Duration").GetValue<float>();

        if (vengefulStanceRoutine != null)
        {
            Debug.Log("Reset VengefulStance");
            StopCoroutine(vengefulStanceRoutine);
            gameUnit.HealthHandler.OnHit.RemoveAllListeners();
            gameUnit.Damager.RemoveDamageReduction(abilitySO.abilityAttributie.GetAbilityValueByID("DamageReduction").GetValue<float>());
        }
        
        vengefulStanceRoutine = StartCoroutine(Utility.TimedAbility
            
        (abilitySO, () =>
        {
            Debug.Log("Start VengefulStance");

            PopUpTextManager.Instance.PopUpText(transform, abilitySO.abilityAttributie.GetAbilityValueByID("Visual").GetValue<string>(), Color.red);
            
            gameUnit.CanMove(false);
            gameUnit.Damager.AddDamageReduction(abilitySO.abilityAttributie.GetAbilityValueByID("DamageReduction").GetValue<float>());

            damageInfo.owner = gameUnit;
            damageInfo.damageType = DamageInfo.DamageType.Melee;

            gameUnit.HealthHandler.OnHit.AddListener(damageInfo =>
            {
                damageInfo.damageAmount = Utility.CalculatePercentageOfValue(damageInfo.damageAmount, abilitySO.abilityAttributie.GetAbilityValueByID("Damage Return").GetValue<float>());
                Debug.Log(damageInfo.owner);
                damageInfo.owner.HealthHandler.TakeDamage(damageInfo);
            });;
        }

        , duration, (abilitySO) =>
        {
            Debug.Log("End VengefulStance");

            gameUnit.CanMove(true);
            gameUnit.Damager.RemoveDamageReduction(abilitySO.abilityAttributie.GetAbilityValueByID("DamageReduction").GetValue<float>());
            gameUnit.HealthHandler.OnHit.RemoveAllListeners();
            vengefulStanceRoutine = null;
        }));
    }
    
    public void MyrmidonSlash(AbilitySO abilitySO)
    {
        Debug.Log("MyrmidonSlash");
        PopUpTextManager.Instance.PopUpText(transform, abilitySO.abilityAttributie.GetAbilityValueByID("Visual").GetValue<string>(), Color.red);
        gameUnit.Damager.CanDoubleDamage();
    }
}
