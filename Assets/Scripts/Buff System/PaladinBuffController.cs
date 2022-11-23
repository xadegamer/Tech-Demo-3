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
            case PaladinBuff.JudgementOfRighteousness: return JudgementOfWeaknessBuff(buffSO, target);
            case PaladinBuff.JudgementofWisdom: return JudgementOfWeaknessBuff(buffSO, target);
            case PaladinBuff.JudgementofWeakness: return JudgementOfWeaknessBuff(buffSO, target);
            default: return null;
        }
    }

    public Buff JudgementOfWeaknessBuff(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found JudgementOfWeaknessBuff");
        
        float damageReduction = buffSO.buffbuffAttributes.GetAbilityValueByID("MeleeAttackDamageReduc").GetValue<float>();
        
        //float timer = 2;

        return new Buff(buffSO, target, () =>
        { //Start
            target.Damager.AddDamageReduction(damageReduction);
        }
        , () =>
        {// In Progress
            
            //if(timer > 0) timer -= Time.deltaTime;
            //else
            //{
            //    damageInfo.damageAmount = 5;
            //    target.HealthHandler.TakeDamage(damageInfo);
            //    timer = 2;
            //}
            
        }, () =>
        { // End
            target.Damager.RemoveDamageReduction(damageReduction);
        });
    }
}
