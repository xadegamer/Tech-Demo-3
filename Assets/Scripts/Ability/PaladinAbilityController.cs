using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinAbilityController : GameUnitAbilityController
{
    [SerializeField] AbilitySO currentJudgement;

    protected void Start()
    {
        AbilityUIManager.Instance.SetAbilities(abilitySetSOArray);
    }

    protected override void AssignSetAbilityActions(AbilitySetSO abilitySetSO)
    {
        foreach (AbilitySO abilitySO in abilitySetSO.abilities)
        {
            switch (abilitySO.GetAbilityType<PaladinAbilities>())
            {
                case PaladinAbilities.CrusaderStrike: abilitySO.SetAbilityAction(CrusaderStrike); break;
                case PaladinAbilities.HammerofJustice: abilitySO.SetAbilityAction(HammerofJustice); break;
                case PaladinAbilities.DivineStorm: abilitySO.SetAbilityAction(DivineStorm); break;
                case PaladinAbilities.Judgement: abilitySO.SetAbilityAction(Judgement); break;
                case PaladinAbilities.JudgementOfRighteousness: abilitySO.SetAbilityAction(JudgementOfRighteousness); break;
                case PaladinAbilities.JudgementofWisdom: abilitySO.SetAbilityAction(JudgementOfWisdom); break;
                case PaladinAbilities.JudgementofWeakness: abilitySO.SetAbilityAction(JudgementOfWeakness); break;
                default: break;
            }
        }
    }

    public void CrusaderStrike(AbilitySO  abilitySO)
    {
        float damage = Utility.CalculateValueWithPercentage(gameUnit.GetStat().GetCharacterClassSO().minbaseDamage, abilitySO.abilityData.GetAbilityValueByID("BasePhysicalDamage").GetValue(), true);

        damageInfo.SetUp(DamageInfo.DamageType.Melee, damage, false, false);
        
        PlayerUnit.Instance.GetTarget().GetComponent<HealthHandler>().TakeDamage(damageInfo);

        Debug.Log("Did Crusader Strike : " + damage);
    }

    public void HammerofJustice(AbilitySO abilitySO)
    {
        float stunDuration = abilitySO.abilityData.GetAbilityValueByID("StunDuration").GetValue();
        Debug.Log("Did Hammer of Justice : " + stunDuration);
    }

    public void DivineStorm(AbilitySO abilitySO)
    {
        float damage = Utility.CalculateValueWithPercentage(gameUnit.GetStat().GetCharacterClassSO().minbaseDamage, abilitySO.abilityData.GetAbilityValueByID("BasePhysicalDamage").GetValue(), true);

        damageInfo.SetUp(DamageInfo.DamageType.Melee, damage, false, false);

        float healAmount = Utility.CalculatePercentageOfValue(PlayerUnit.Instance.GetTarget().GetComponent<HealthHandler>().TakeDamage(damageInfo), abilitySO.abilityData.GetAbilityValueByID("Heal").GetValue());
        Debug.Log("Did Divine Storm : " + damage);
        Debug.Log("Heal : " + healAmount);


        GetComponent<HealthHandler>().RestoreHealth(healAmount);
    }

    public void Judgement(AbilitySO abilitySO)
    {
        float damage = Utility.CalculatePercentageOfValue(gameUnit.GetStat().GetCharacterClassSO().minbaseDamage, abilitySO.abilityData.GetAbilityValueByID("BaseWeaponDamage").GetValue());

        damageInfo.SetUp(DamageInfo.DamageType.Melee, damage, false, false);
        PlayerUnit.Instance.GetTarget().GetComponent<HealthHandler>().TakeDamage(damageInfo);
        Debug.Log("Did Judgement : " + damage);

        buffManager.SendBuff(currentJudgement.buff, PlayerUnit.Instance.GetTarget());
        Debug.Log("Do " + currentJudgement.abilityName);
    }

    public void JudgementOfRighteousness(AbilitySO abilitySO)
    {
        currentJudgement = abilitySO;
        Debug.Log("Selected Judgement of Righteousness");
    }

    public void JudgementOfWisdom(AbilitySO abilitySO)
    {
        currentJudgement = abilitySO;
        Debug.Log("Selected Judgement of Wisdom");
    }
    
    public void JudgementOfWeakness(AbilitySO abilitySO)
    {
        currentJudgement = abilitySO;
        Debug.Log("Selected Judgement of Weakness");
    }

    public void HolyLight(AbilitySO abilitySO)
    {
        Debug.Log("Do Holy Light");
    }
}
