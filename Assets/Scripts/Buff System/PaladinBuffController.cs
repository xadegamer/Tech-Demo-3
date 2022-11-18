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

    public override bool AddBuff(Buff newBuff)
    {
        if (base.AddBuff(newBuff))
        {
            UIManager.Instance.GetPlayerrUI().AddBuff(newBuff);
            return true;
        }
        return false;
    }

    public Buff JudgementOfWeaknessBuff(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found JudgementOfWeaknessBuff");
        
        float damageReduction = buffSO.buffData.GetAbilityValueByID("MeleeAttackDamage").GetValue();
        
        DamageInfo damageInfo = new DamageInfo();

        float timer = 2;

        return new Buff(buffSO, target, () =>
        { //Start
            target.Damager.SetDamageReducion(damageReduction);
        }
        , () =>
        {// In Progress
            if(timer > 0) timer -= Time.deltaTime;
            else
            {
                damageInfo.damageAmount = 5;
                target.HealthHandler.TakeDamage(damageInfo);
                timer = 2;
            }
        }, () =>
        { // End
            target.Damager.SetDamageReducion(0f);
        });
    }


}
