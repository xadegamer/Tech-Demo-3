using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIManager : MonoBehaviour
{
    public static AbilityUIManager Instance { get; private set; }

    [Header("Abilities")]
    [SerializeField] private AbilitySetUI[] abilitySetUI;


    [Header(header: "CastBar")]
    [SerializeField]private GameObject castBar;
    [SerializeField] private Image castBarFill;
    [SerializeField] private TextMeshProUGUI castBarText;

    private AbilitySetUI currentAbilitySetUI;
    void Awake()
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

    public void ActivateCastBar(string abilityName)
    {
        castBarText.text = abilityName;
    }

    public void SetCastBar(float castDuration)
    {
        castBarFill.fillAmount = castDuration;
        castBar.SetActive(true);
    }

    public void GlobalCooldown()
    {
        foreach (AbilitySetUI item in abilitySetUI)
        {
            foreach(AbilityHolderUI abilityHolderUI in item.GetAbilityHolderUIList())
            {
                //if (abilityHolderUI.GetCurrentAbilitySO().type == AbilitySO.Type.InstantCast)
                abilityHolderUI.ActivateGlobalCooldown();
            }
        }
    }
}
