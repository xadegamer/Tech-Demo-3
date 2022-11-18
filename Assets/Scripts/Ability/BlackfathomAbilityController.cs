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

    public void BlackfathomHamstring(AbilitySO abilitySO)
    {
        Debug.Log("BlackfathomHamstring");
    }

    public void Bash(AbilitySO abilitySO)
    {
        Debug.Log("Bash");
    }

    public void VengefulStance (AbilitySO abilitySO)
    {
        Debug.Log("VengefulStance");
    }

    public void MyrmidonSlash(AbilitySO abilitySO)
    {
        Debug.Log("MyrmidonSlash");
    }
}
