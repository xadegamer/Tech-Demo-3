using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackfathomAbilityController : GameUnitAbilityController
{
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            abilitySetSOArray[0].abilities[0].UseAbility();
        }
    }

    public void BlackfathomHamstring(AbilitySO abilitySO)
    {
        float damage = Utility.CalculateValueWithPercentage(gameUnit.GetStat().GetCharacterClassSO().minbaseDamage, abilitySO.abilityData.GetAbilityValueByID("DamageInc").GetValue(), true);
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
        float duration = 0;
        StartCoroutine(Utility.TimedAbility(abilitySO, () => Debug.Log("Start VengefulStance"), duration, null));
    }

    public void MyrmidonSlash(AbilitySO abilitySO)
    {
        // Do double damage
        Debug.Log("MyrmidonSlash");
    }
}
