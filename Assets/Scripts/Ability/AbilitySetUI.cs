using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySetUI : MonoBehaviour
{
    [SerializeField] private Vector2 hidePos;
    [SerializeField] private AbilityHolderUI abilityPrefab;

    [SerializeField] private List<AbilityHolderUI> abilityHolderUIList;

    private RectTransform abilityHolder;
    private void Awake()
    {
        abilityHolder = GetComponent<RectTransform>();
    }

    public void ShowAbilitySet() => abilityHolder.anchoredPosition = Vector2.zero;

    public void HideAbilitySet() => abilityHolder.anchoredPosition = hidePos;

    public void SpawnAndSetAbility(AbilitySOSet abilitySOSet)
    {
        for (int i = 0; i < abilitySOSet.abilities.Length; i++)
        {
            AbilityHolderUI abilityHolderUI = Instantiate(abilityPrefab, abilityHolder);
            abilityHolderUI.SetAbility(abilitySOSet.abilities[i]);
            abilityHolderUIList.Add(abilityHolderUI);
        } 
    }
}