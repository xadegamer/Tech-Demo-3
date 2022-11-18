using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AbilitySetUI : MonoBehaviour
{
    [SerializeField] private AbilityHolderUI abilityPrefab;
    [SerializeField] private List<AbilityHolderUI> abilityHolderUIList;

    public void ShowAbilitySet()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        transform.SetAsLastSibling();
    }

    public void HideAbilitySet()
    {
        abilityHolderUIList.ForEach(x => x.Disable());      
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().interactable = false;
    } 
    
    public void SpawnAndSetAbility(AbilitySetSO abilitySetSO)
    {
        for (int i = 0; i < abilitySetSO.abilities.Length; i++)
        {
            AbilityHolderUI abilityHolderUI = Instantiate(abilityPrefab, transform);
            
            abilityHolderUIList.Add(abilityHolderUI);

            abilityHolderUI.SetCurrentAbility(abilitySetSO.abilities[i]);
            
            if (abilitySetSO.abilities[i].connectedAbilities.Length > 0) abilityHolderUI.SetConnectedAbilities(abilitySetSO.abilities[i].connectedAbilities);
        } 
    }

    public List<AbilityHolderUI> GetAbilityHolderUIList()
    {
        return abilityHolderUIList;
    }
}