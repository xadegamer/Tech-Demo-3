using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AbilityUIManager : MonoBehaviour
{
    public static AbilityUIManager Instance { get; private set; }

    [SerializeField] private CanvasGroup abilityPanelHolder;
    [SerializeField] private GameObject attackButton;

    [Header("Abilities")]
    [SerializeField] private AbilitySetUI[] abilitySetUI;

    [Header(header: "CastBar")]
    [SerializeField]private GameObject castBar;
    [SerializeField] private Image castBarFill;
    [SerializeField] private TextMeshProUGUI castBarText;

    private AbilitySetUI currentAbilitySetUI;
    private GameUnit owner;
    
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

        PlayerUnit.Instance.OnTargetFound += ToggleActive;
        PlayerUnit.Instance.OnStun += ToggleUseability;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.RightArrow)) ShowAbililitySetUI(abilitySetUI[0]);
        //if (Input.GetKeyDown(KeyCode.LeftArrow)) ShowAbililitySetUI(abilitySetUI[1]);
        //if (Input.GetKeyDown(KeyCode.UpArrow)) ShowAbililitySetUI(abilitySetUI[2]);
        //if (Input.GetKeyDown(KeyCode.DownArrow)) ShowAbililitySetUI(abilitySetUI[3]);

        if (Input.GetKeyDown(KeyCode.RightArrow)) ShowAbililitySetUI(abilitySetUI[0]);
        if (Input.GetKeyDown(KeyCode.DownArrow)) ShowAbililitySetUI(abilitySetUI[1]);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ShowAbililitySetUI(abilitySetUI[2]);
        if (Input.GetKeyDown(KeyCode.UpArrow)) ShowAbililitySetUI(abilitySetUI[3]);
    }

    public void ToggleActive(bool toggle)
    {
        abilityPanelHolder.alpha = toggle ? 1 : 0;
        abilityPanelHolder.interactable = toggle;
    }

    private void SwapHandler_OnRightSwipe(object sender, EventArgs e)
    {
        ShowAbililitySetUI(abilitySetUI[0]);
    }

    private void SwapHandler_OnLeftSwipe(object sender, EventArgs e)
    {
        ShowAbililitySetUI(abilitySetUI[2]);
    }

    private void SwapHandler_OnUpSwipe(object sender, EventArgs e)
    {
        ShowAbililitySetUI(abilitySetUI[3]);
    }

    private void SwapHandler_OnDownSwipe(object sender, EventArgs e)
    {
        ShowAbililitySetUI(abilitySetUI[1]);
    }

    public void ShowAbililitySetUI(AbilitySetUI abilitySetUI)
    {
        if (abilitySetUI == currentAbilitySetUI) return;
        if (currentAbilitySetUI != null) currentAbilitySetUI.HideAbilitySet();
        currentAbilitySetUI = abilitySetUI;
        currentAbilitySetUI.ShowAbilitySet();
    }

    public void SetAbilities(GameUnit owner, List<AbilitySet> abilitySet)
    {
        this.owner = owner;
        for (int i = 0; i < abilitySet.Count; i++) abilitySetUI[i].SpawnAndSetAbility(abilitySet[i]);
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
        abilitySetUI.ToList().ForEach(item => item.GetAbilityHolderUIList().Where(x => x.GetCurrentAbility().abilitySO.type != AbilitySO.Type.SetUp).ToList().ForEach(x => x.ActivateGlobalCooldown()));
    }

    public void ToggleMeleeAbilities(bool toggle)
    {
        abilitySetUI.ToList().ForEach(item => item.GetAbilityHolderUIList().Where(x => x.GetCurrentAbility().abilitySO.range == AbilitySO.Range.Melee).ToList().ForEach(x => x.ToggleUseAbility(toggle)));
    }
    public void ToggleUseability(bool toggle)
    {
        abilitySetUI.ToList().ForEach(item => item.GetAbilityHolderUIList().ToList().ForEach(x => x.ToggleUseAbility(!toggle)));
    }

    public GameUnit GetOwner() => owner;
}
