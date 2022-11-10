using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Player")]
    [SerializeField] private CharacterUI playerUI;

    [Header("Enemy")]
    [SerializeField] private CharacterUI enemyUI;

    [Header("TargetOfTarget")]
    [SerializeField] private CharacterUI targetOfTargetUI;

    [Header("Abilities")]
    [SerializeField] private AbilitySetUI[]  abilitySetUI;

    private AbilitySetUI currentAbilitySetUI;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ScreenSwapHandler.Instance.OnDownSwipe += SwapHandler_OnDownSwipe;
        ScreenSwapHandler.Instance.OnUpSwipe += SwapHandler_OnUpSwipe;
        ScreenSwapHandler.Instance.OnLeftSwipe += SwapHandler_OnLeftSwipe;
        ScreenSwapHandler.Instance.OnRightSwipe += SwapHandler_OnRightSwipe;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) ShowAbililitySetUI(abilitySetUI[0]);
        
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ShowAbililitySetUI(abilitySetUI[1]);

        if (Input.GetKeyDown(KeyCode.UpArrow)) ShowAbililitySetUI(abilitySetUI[2]);

        if (Input.GetKeyDown(KeyCode.DownArrow)) ShowAbililitySetUI(abilitySetUI[3]);
    }

    private void SwapHandler_OnRightSwipe(object sender, EventArgs e)
    {
        ShowAbililitySetUI(abilitySetUI[0]);
    }

    private void SwapHandler_OnLeftSwipe(object sender, EventArgs e)
    {
        ShowAbililitySetUI(abilitySetUI[1]);
    }

    private void SwapHandler_OnUpSwipe(object sender, EventArgs e)
    {
        ShowAbililitySetUI(abilitySetUI[2]);
    }

    private void SwapHandler_OnDownSwipe(object sender, EventArgs e)
    {
        ShowAbililitySetUI(abilitySetUI[3]);
    }

    public void ShowAbililitySetUI(AbilitySetUI abilitySetUI)
    {
        if (abilitySetUI == currentAbilitySetUI) return;
        if (currentAbilitySetUI != null) currentAbilitySetUI.HideAbilitySet();
        currentAbilitySetUI = abilitySetUI;
        currentAbilitySetUI.ShowAbilitySet();
    }

    public void SetAbilities(AbilitySOSet[] abilitySOSets)
    {
        for (int i = 0; i < abilitySetUI.Length; i++) abilitySetUI[i].SetAbility(abilitySOSets[i]);
    }
}


[Serializable]
public class CharacterUI
{
    [SerializeField] private Image enemyIcon;
    [SerializeField] private Image enemyHealthBar;
    [SerializeField] private Image enemyManaBar;
    [SerializeField] private Transform buffHolder;

    public void SetEnemyIcon(Sprite icon) { enemyIcon.sprite = icon; }
    public void SetEnemyHealthBar(float health) { enemyHealthBar.fillAmount = health; }
    public void SetEnemyManaBar(float mana) { enemyManaBar.fillAmount = mana; }
}

[Serializable]
public class AbilitySetUI
{
    [SerializeField] private RectTransform abilityHolder;
    [SerializeField] private Vector2 hidePos;
    [SerializeField] private AbilityHolderUI[] abilityHolderUIList;

    public void ShowAbilitySet() => abilityHolder.anchoredPosition = Vector2.zero;

    public void HideAbilitySet() => abilityHolder.anchoredPosition = hidePos;

    public void SetAbility(AbilitySOSet abilitySOSet)
    {
        for (int i = 0; i < abilityHolderUIList.Length; i++) abilityHolderUIList[i].SetAbility(abilitySOSet.abilities[i]);
    }
}