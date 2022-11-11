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

    [Header(header: "CastBar")]
    [SerializeField] private Image castBarFill;
    [SerializeField] private TextMeshProUGUI castBarText;

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
        for (int i = 0; i < abilitySetUI.Length; i++)
        {
            abilitySetUI[i].SpawnAndSetAbility(abilitySOSets[i]);
        }
    }

    public CharacterUI GetPlayerrUI() => playerUI;
    public CharacterUI GetEnemyUI() => enemyUI;
    public CharacterUI GetTargetOfTargetUI() => targetOfTargetUI;

    public void SetCastBarText(string text)
    {
        castBarText.text = text;
    }

    public void SetCastBarFill(float fillAmount)
    {
        castBarFill.fillAmount = fillAmount;
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