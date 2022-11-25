using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PaladinBuffController : GameUnitBuffController
{
    protected override Buff CreateBuff(BuffSO buffSO, GameUnit target)
    {
        switch (buffSO.GetBuffType<PaladinBuff>())
        {
            case PaladinBuff.DevotionAura: return DevotionAura(buffSO, target);
            case PaladinBuff.MagicalAura: return MagicalAura(buffSO, target);
            case PaladinBuff.RetributionAura: return RetributionAura(buffSO, target);

            case PaladinBuff.SealOfRighteousness: return SealOfRighteousness(buffSO, target);
            case PaladinBuff.SealOfLight: return SealOfLight(buffSO, target);
            case PaladinBuff.SealOfJustice: return SealOfJustice(buffSO, target);

            case PaladinBuff.JudgementOfRighteousness: return JudgementOfRighteousness(buffSO, target);
            case PaladinBuff.JudgementOfWisdom: return JudgementOfWisdom(buffSO, target);
            case PaladinBuff.JudgementOfWeakness: return JudgementOfWeakness(buffSO, target);

            case PaladinBuff.BlessingOfMight: return BlessingOfMight(buffSO, target);
            case PaladinBuff.BlessingOfWisdom: return BlessingOfWisdom(buffSO, target);
            case PaladinBuff.BlessingOfKings: return BlessingOfKings(buffSO, target);
            default: return null;
        }
    }

    public Buff DevotionAura(BuffSO buffSO, GameUnit target)
    {
        float allDamageResistance = buffSO.buffbuffAttributes.GetAbilityValueByID("AllDamageResistance").GetValue<float>();

        return new Buff(buffSO, target, () =>
        { //Start
            target.HealthHandler.ModifyAllDamageResistance(allDamageResistance);
        }
        , null, () =>
        { // End
            target.HealthHandler.ModifyAllDamageResistance(allDamageResistance,false);
        });
    }
    public Buff MagicalAura(BuffSO buffSO, GameUnit target)
    {      
        float interval = buffSO.buffbuffAttributes.GetAbilityValueByID("Interval").GetValue<float>();
        float manaAmount = Utility.CalculatePercentageOfValue(gameUnit.GetCharacterClassSO().mana, buffSO.buffbuffAttributes.GetAbilityValueByID("BaseMana").GetValue<float>());

        float timer = interval;

        return new Buff(buffSO, target, null
        , () =>
        {// In Progress

            if (timer > 0) timer -= Time.deltaTime;
            else
            {
                (target as PlayerUnit).PlayerStatHandler.GetManaValue().IncreaseValue(manaAmount);
                timer = interval;
            }

        }, null);
    }
    public Buff RetributionAura(BuffSO buffSO, GameUnit target)
    {
        void DoAura(DamageInfo damageInfo)
        {
            float damage = Utility.CalculatePercentageOfValue(gameUnit.GetCharacterClassSO().maxbaseDamage, buffSO.buffbuffAttributes.GetAbilityValueByID("BaseMeleeDamage").GetValue<float>());
            DamageInfo newDamageInfo = new DamageInfo(gameUnit);
            newDamageInfo.SetUp(DamageInfo.DamageType.Holy, damage, false);
            newDamageInfo.reflect = true;
            damageInfo.owner.HealthHandler.TakeDamage(newDamageInfo);
        }

        return new Buff(buffSO, target, () =>
        { //Start
            target.HealthHandler.OnHit.AddListener(DoAura);
        }
        , null, () =>
        { // End
            target.HealthHandler.OnHit.RemoveListener(DoAura);
        });
    }
   
    public Buff SealOfLight(BuffSO buffSO, GameUnit target)
    {
        float duration = buffSO.buffbuffAttributes.GetAbilityValueByID("Duration").GetValue<float>();

        return new Buff(buffSO, target, () =>
        { //Start
            gameUnit.Damager.OnHit += damageDealth=>
            {
                if (Utility.CalculateChance(buffSO.buffbuffAttributes.GetAbilityValueByID("HealChance").GetValue<float>()))
                {
                    float health = Utility.CalculatePercentageOfValue(damageDealth, buffSO.buffbuffAttributes.GetAbilityValueByID("Heal").GetValue<float>());
                    gameUnit.HealthHandler.RestoreHealth(health);
                }
            };
        }
        , null, () =>
        { // End
            gameUnit.Damager.OnHit = null;
        });
    }
    public Buff SealOfJustice(BuffSO buffSO, GameUnit target)
    {
        float duration = buffSO.buffbuffAttributes.GetAbilityValueByID("Duration").GetValue<float>();

        return new Buff(buffSO, target, () =>
        { //Start

            target.Damager.OnHitTargetHealth += healthHandler =>
            {
                if (Utility.CalculateChance(buffSO.buffbuffAttributes.GetAbilityValueByID("StunChance").GetValue<float>()))
                {
                    float stunDuration = buffSO.buffbuffAttributes.GetAbilityValueByID("StunDuration").GetValue<float>();
                    healthHandler.GetComponent<GameUnit>().Stun(stunDuration);
                }
            };          
        }
        , null, () =>
        { // End
            target.Damager.OnHitTargetHealth = null;
        });
    }
    public Buff SealOfRighteousness(BuffSO buffSO, GameUnit target)
    {
        float duration = buffSO.buffbuffAttributes.GetAbilityValueByID("Duration").GetValue<float>();

        return new Buff(buffSO, target, () =>
        { //Start
            gameUnit.Damager.OnHitTargetHealth += healthHandler =>
            {
                float damage = Utility.CalculatePercentageOfValue(gameUnit.GetCharacterClassSO().maxbaseDamage, buffSO.buffbuffAttributes.GetAbilityValueByID("BaseWeaponDamage").GetValue<float>());
                DamageInfo newDamageInfo = new DamageInfo(gameUnit);
                newDamageInfo.SetUp(DamageInfo.DamageType.Holy, damage, false);
                if (healthHandler) healthHandler.TakeDamage(newDamageInfo);
            };
        }
        ,null, () =>
        { // End
            gameUnit.Damager.OnHitTargetHealth = null;
        });
    }

    public Buff JudgementOfRighteousness(BuffSO buffSO, GameUnit target)
    {
        void RestoreTargetHealth(DamageInfo damageInfo)
        {
            if (Utility.CalculateChance(buffSO.buffbuffAttributes.GetAbilityValueByID("HealChance").GetValue<float>()))
            {
                float health = Utility.CalculatePercentageOfValue(damageInfo.owner.GetCharacterClassSO().health, buffSO.buffbuffAttributes.GetAbilityValueByID("Health").GetValue<float>());
                damageInfo.owner.HealthHandler.RestoreHealth(health);
            }
        }

        return new Buff(buffSO, target, () =>
        { //Start
            target.HealthHandler.OnHit.AddListener(RestoreTargetHealth);
        }
        , null, () =>
        { // End
            target.HealthHandler.OnHit.RemoveListener(RestoreTargetHealth);
        });
    }
    public Buff JudgementOfWisdom(BuffSO buffSO, GameUnit target)
    {
        void RestoreTargetMana(DamageInfo damageInfo)
        {
            if (Utility.CalculateChance(buffSO.buffbuffAttributes.GetAbilityValueByID("ManaChance").GetValue<float>()))
            {
                float mana = Utility.CalculatePercentageOfValue(damageInfo.owner.GetCharacterClassSO().health, buffSO.buffbuffAttributes.GetAbilityValueByID("mana").GetValue<float>());
                (damageInfo.owner as PlayerUnit).PlayerStatHandler.GetManaValue().IncreaseValue(mana);
            }
        }

        return new Buff(buffSO, target, () =>
        { //Start
            target.HealthHandler.OnHit.AddListener(RestoreTargetMana);
        }
        , null, () =>
        { // End
            target.HealthHandler.OnHit.RemoveListener(RestoreTargetMana);
        });
    }
    public Buff JudgementOfWeakness(BuffSO buffSO, GameUnit target)
    {
        float damageReduction = buffSO.buffbuffAttributes.GetAbilityValueByID("MeleeAttackDamageReduc").GetValue<float>();

        return new Buff(buffSO, target, () =>
        { //Start
            target.Damager.ModifyDamageReduction(damageReduction);
        }
        , null, () =>
        { // End
            target.Damager.ModifyDamageReduction(damageReduction, false);
        });
    }

    public Buff BlessingOfMight(BuffSO buffSO, GameUnit target)
    {
        float damageIncrease = buffSO.buffbuffAttributes.GetAbilityValueByID("ExtraDamage").GetValue<float>();

        return new Buff(buffSO, target, () =>
        { //Start
            target.Damager.ModifyExtraDamage(damageIncrease);
        }
        , null, () =>
        { // End
            target.Damager.ModifyExtraDamage(damageIncrease, false);
        });
    }
    public Buff BlessingOfWisdom(BuffSO buffSO, GameUnit target)
    {
        float mana = buffSO.buffbuffAttributes.GetAbilityValueByID("Mana").GetValue<float>();

        float timer = buffSO.buffbuffAttributes.GetAbilityValueByID("Interval").GetValue<float>();

        return new Buff(buffSO, target, null
        , () =>
        {// In Progress

            if (timer > 0) timer -= Time.deltaTime;
            else
            {
                (target as PlayerUnit).PlayerStatHandler.GetManaValue().IncreaseValue(mana);
                timer = buffSO.buffbuffAttributes.GetAbilityValueByID("Interval").GetValue<float>();
            }

        },null);
    }
    public Buff BlessingOfKings(BuffSO buffSO, GameUnit target)
    {
        float damage = buffSO.buffbuffAttributes.GetAbilityValueByID("AdditionalBaseDamage").GetValue<float>();

        float attackSpeedIncrease = buffSO.buffbuffAttributes.GetAbilityValueByID("AttackSpeedInc").GetValue<float>();

        float critIncrease = buffSO.buffbuffAttributes.GetAbilityValueByID("CritInc").GetValue<float>();

        float manaAndHealthIncrease = buffSO.buffbuffAttributes.GetAbilityValueByID("MaxManaHealthInc").GetValue<float>();

        return new Buff(buffSO, target, () =>
        { //Start
            target.Damager.ModifyExtraDamage(damage);
            target.GetStat().ModifyAttackSpeed(Utility.CalculatePercentageOfValue(target.GetCharacterClassSO().attackSpeed, attackSpeedIncrease), false);
            target.HealthHandler.ModifyMaxHealth(Utility.CalculatePercentageOfValue(target.GetCharacterClassSO().health, manaAndHealthIncrease), true);
            target.Damager.ModifyChanceToHit(Utility.CalculatePercentageOfValue(target.GetCharacterClassSO().chanceToHit, critIncrease), true);
            (target as PlayerUnit).PlayerStatHandler.GetManaValue().ModifyMaxValue(Utility.CalculatePercentageOfValue(target.GetCharacterClassSO().mana, manaAndHealthIncrease), true);
        }
        , null, () =>
        { // End
            target.Damager.ModifyDamageReduction(damage, false);
            target.GetStat().ModifyAttackSpeed(Utility.CalculatePercentageOfValue(target.GetCharacterClassSO().attackSpeed, attackSpeedIncrease), true);
            target.HealthHandler.ModifyMaxHealth(Utility.CalculatePercentageOfValue(target.GetCharacterClassSO().health, manaAndHealthIncrease), false);
            target.Damager.ModifyChanceToHit(Utility.CalculatePercentageOfValue(target.GetCharacterClassSO().chanceToHit, critIncrease), false);
            (target as PlayerUnit).PlayerStatHandler.GetManaValue().ModifyMaxValue(Utility.CalculatePercentageOfValue(target.GetCharacterClassSO().mana, manaAndHealthIncrease), false);    
        });
    }
}
