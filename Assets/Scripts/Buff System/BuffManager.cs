using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [SerializeField] private BuffSO[] buffs;
    private GameUnit target;

    protected void AssignBuffActions()
    {
        foreach (BuffSO buff in buffs)
        {
            if(buff.buffType == BuffType.Permanent)
            {
                JudgementOfWeaknessDebuff(buff);
            }

            
        }
    }

    public void JudgementOfWeaknessDebuff(BuffSO buffSO)
    {
        float damageReduction = buffSO.buffData.GetAbilityValueByID("MeleeAttackDamage").GetValue();

        void OnStart()
        {
            target.Damager.SetDamageReducion(damageReduction);
        }

        void InProgress()
        {
            target.Damager.SetDamageReducion(0f);
        }

        void OnEnd()
        {
            target.Damager.SetDamageReducion(0f);
        }

        buffSO.SetActions(OnStart, InProgress, OnEnd);
    }
}
