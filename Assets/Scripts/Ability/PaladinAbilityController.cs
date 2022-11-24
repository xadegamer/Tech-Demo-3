using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinAbilityController : GameUnitAbilityController
{
    [SerializeField] Ability currentJudgement;

    [SerializeField] Ability currentAura;

    [SerializeField] Ability currentSeal;

    Coroutine sealCoroutine;

    private bool sealActive;

    private bool auraActive;


    protected void Start()
    {
        AbilityUIManager.Instance.SetAbilities(gameUnit, abilities);
        currentAura.UseAbility();
        currentSeal.UseAbility();
    }

    protected override void AssignSetAbilityActions(AbilitySet abilitySet)
    {
        foreach (Ability ability in abilitySet.abilities)
        {
            AssignActions(ability);
            for (int i = 0; i < ability.connectedAbilities.Count; i++) AssignActions(ability.connectedAbilities[i]);
        }
    }

    protected void AssignActions(Ability ability)
    {
        switch (ability.abilitySO.GetAbilityType<PaladinAbilities>())
        {
            case PaladinAbilities.CrusaderStrike: ability.SetAbilityAction(CrusaderStrike); break;
            case PaladinAbilities.HammerofJustice: ability.SetAbilityAction(HammerofJustice); break;
            case PaladinAbilities.DivineStorm: ability.SetAbilityAction(DivineStorm); break;
            case PaladinAbilities.Judgement: ability.SetAbilityAction(Judgement); break;


            case PaladinAbilities.DevotionAura: ability.SetAbilityAction(DevotionAura); currentAura = ability; break;
            case PaladinAbilities.MagicalAura: ability.SetAbilityAction(MagicalAura); break;
            case PaladinAbilities.RetributionAura: ability.SetAbilityAction(RetributionAura); break;

            case PaladinAbilities.SealOfRighteousness: ability.SetAbilityAction(SealOfRighteousness); currentSeal = ability; break;
            case PaladinAbilities.SealOfLight: ability.SetAbilityAction(SealOfLight); break;
            case PaladinAbilities.SealOfJustice: ability.SetAbilityAction(SealOfJustice); break;

            case PaladinAbilities.JudgementOfRighteousness: ability.SetAbilityAction(JudgementOfRighteousness); currentJudgement = ability; break;
            case PaladinAbilities.JudgementofWisdom: ability.SetAbilityAction(JudgementOfWisdom); break;
            case PaladinAbilities.JudgementofWeakness: ability.SetAbilityAction(JudgementOfWeakness); break;

            case PaladinAbilities.BlessingOfMight: ability.SetAbilityAction(BlessingOfMight); break;
            case PaladinAbilities.BlessingOfWisdom: ability.SetAbilityAction(BlessingOfWisdom); break;
            case PaladinAbilities.BlessingOfKings: ability.SetAbilityAction(BlessingOfKings); break;

            default: break;
        }
    }

    #region Set 1
    public void CrusaderStrike(Ability ability)
    {
        float damage = Utility.CalculateValueWithPercentage(gameUnit.GetStat().GetCharacterClassSO().minbaseDamage, ability.abilitySO.abilityAttributie.GetAbilityValueByID("BasePhysicalDamage").GetValue<float>(), true);

        damageInfo.SetUp(DamageInfo.DamageType.Physical, damage, false, false);

        gameUnit.GetTarget().HealthHandler.TakeDamage(damageInfo);

        Debug.Log("Did Crusader Strike : " + damage);
    }

    public void HammerofJustice(Ability ability)
    {
        float stunDuration = ability.abilitySO.abilityAttributie.GetAbilityValueByID("StunDuration").GetValue<float>();
        gameUnit.GetTarget().StartStun();
        Debug.Log("Did Hammer of Justice : " + stunDuration);
    }

    public void DivineStorm(Ability ability)
    {
        float damage = Utility.CalculateValueWithPercentage(gameUnit.GetStat().GetCharacterClassSO().minbaseDamage, ability.abilitySO.abilityAttributie.GetAbilityValueByID("BasePhysicalDamage").GetValue<float>(), true);

        damageInfo.SetUp(DamageInfo.DamageType.Physical, damage, false, false);

        float healAmount = Utility.CalculatePercentageOfValue(PlayerUnit.Instance.GetTarget().GetComponent<HealthHandler>().TakeDamage(damageInfo), ability.abilitySO.abilityAttributie.GetAbilityValueByID("Heal").GetValue<float>());
       
        gameUnit.HealthHandler.RestoreHealth(healAmount);

        Debug.Log("Did Divine Storm : " + damage + " : Heal : " + healAmount);
    }

    public void Judgement(Ability ability)
    {
        float damage = Utility.CalculatePercentageOfValue(gameUnit.GetStat().GetCharacterClassSO().minbaseDamage, ability.abilitySO.abilityAttributie.GetAbilityValueByID("BaseWeaponDamage").GetValue<float>());

        damageInfo.SetUp(DamageInfo.DamageType.Physical, damage, false, false);
        gameUnit.GetTarget().GetComponent<HealthHandler>().TakeDamage(damageInfo);
        Debug.Log("Did Judgement : " + damage);

        buffManager.SendBuff(currentJudgement.abilitySO.buff, PlayerUnit.Instance.GetTarget());
        Debug.Log("Do " + currentJudgement.abilitySO.abilityName);
    }
    #endregion

    #region Set 4

    #region Aura
    public void DevotionAura(Ability ability)
    {
        if (currentAura != null && auraActive) currentAura.buff.RemoveBuff();
        currentAura = ability;
        auraActive = true;
        ability.buff = buffManager.SendBuff(ability.abilitySO.buff, gameUnit);
    }

    public void MagicalAura(Ability ability)
    {
        if (currentAura != null) currentAura.buff.RemoveBuff();
        currentAura = ability;
        ability.buff = buffManager.SendBuff(ability.abilitySO.buff, gameUnit);
    }


    public void RetributionAura(Ability ability)
    {
        if (currentAura != null) currentAura.buff.RemoveBuff();
        currentAura = ability;
        ability.buff = buffManager.SendBuff(ability.abilitySO.buff, gameUnit);
    }
    #endregion

    #region Seal

    public void SealOfRighteousness(Ability ability)
    {
        if(currentSeal != null && sealActive) currentSeal.buff.RemoveBuff();
        currentSeal = ability;
        sealActive = true;
        ability.buff = buffManager.SendBuff(ability.abilitySO.buff, gameUnit);
    }
    
    public void SealOfLight(Ability ability)
    {
        if (currentSeal != null) currentSeal.buff.RemoveBuff();
        currentSeal = ability;
        ability.buff = buffManager.SendBuff(ability.abilitySO.buff, gameUnit);
    }

    public void SealOfJustice(Ability ability)
    {
        if (currentSeal != null) currentSeal.buff.RemoveBuff();
        currentSeal = ability;
        ability.buff = buffManager.SendBuff(ability.abilitySO.buff, gameUnit);
    }
    #endregion

    #region Judgement
    public void JudgementOfRighteousness(Ability ability)
    {
        currentJudgement = ability;
        Debug.Log("Selected Judgement of Righteousness");
    }

    public void JudgementOfWisdom(Ability ability)
    {
        currentJudgement = ability;
        Debug.Log("Selected Judgement of Wisdom");
    }

    public void JudgementOfWeakness(Ability ability)
    {
        currentJudgement = ability;
        Debug.Log("Selected Judgement of Weakness");
    }
    #endregion

    #region Blessing
    public void BlessingOfMight(Ability ability)
    {
        Debug.Log("Do BlessingOfMight");
    }

    public void BlessingOfWisdom(Ability ability)
    {
        Debug.Log("Do BlessingOfWisdom");
    }

    public void BlessingOfKings(Ability ability)
    {
        Debug.Log("Do BlessingOfKings");
    }


    #endregion
    #endregion
}
