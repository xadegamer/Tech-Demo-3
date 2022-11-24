using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using System.Linq;

public enum AbilityState
{
    Ready,
    active,
    OnCooldown,
}

public class AbilityHolderUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IToolTip
{
    [Header("Ability UI")]
    [SerializeField] private Image abilityImage;
    [SerializeField] private Image coolDownSlider;
    [SerializeField] private TextMeshProUGUI abilityCoolDownText;

    [Header("Hold Option")]
    [Range(1.0f, 10.0f)]
    [SerializeField] public float holdDuration = 1.0f;

    [Header("Ability Option")]
    [SerializeField] private AbilityState abilityState;
    [SerializeField] private Ability currentAbility;
    [SerializeField] private List<Ability> connectedAbilityList;

    [Header("Additional Ability Option")]
    [SerializeField] private AbilityHolderUI additionalAbilityPrefab;
    [SerializeField] private Transform additionAbilitiesHolder;
    [SerializeField] private List<AbilityHolderUI> connectedAbilityHolders;
    
    private AbilityHolderUI abilityUIParent;
    private float activeTime;
    private bool buttonHeld;
    Vector2 pos;

    private void Start()
    {
        ResetUI();
    }

    public void SetCurrentAbility(Ability abilitySO)
    {
        abilityImage.sprite = abilitySO.abilitySO.abilityIcon;
        currentAbility = abilitySO;
        currentAbility.abilityHolderUI = this;
    }

    public void SetConnectedAbilities(List<Ability> abilitySOs = null)
    {
        abilitySOs?.ToList().ForEach(x => connectedAbilityList.Add(x));
    }

    public Ability GetCurrentAbility() => currentAbility;

    public AbilityState GetAbilityState() => abilityState;

    public void Use()
    {
        if (buttonHeld)  {buttonHeld = false; return;} 
        
        if (abilityState == AbilityState.Ready)
        {        
            switch (currentAbility.abilitySO.type)
            {
                case AbilitySO.Type.InstantCast:
                    if (AbilityUIManager.Instance.GetOwner().TryUseAbility(currentAbility))
                    {
                        ActivateNormalCooldown();
                        AbilityUIManager.Instance.GlobalCooldown();
                    } 
                    break;
                case AbilitySO.Type.DelayedCast:
                    if (PlayerUnit.Instance.TryUseAbility(currentAbility)) abilityState = AbilityState.active;
                    break;
                case AbilitySO.Type.SetUp:

                    if (!abilityUIParent)
                    {
                        ToggleConnectedAbilitiesUI(!additionAbilitiesHolder.gameObject.activeInHierarchy);
                    }
                    else
                    {
                        currentAbility.UseAbility();
                        abilityUIParent.SwapAbility(currentAbility);
                        abilityUIParent.ToggleConnectedAbilitiesUI(false);
                       // AbilityUIManager.Instance.GlobalCooldown();
                    }
                    break;
                case AbilitySO.Type.SetUpAndInstantCast:
                    if (!abilityUIParent)
                    {
                        if (additionAbilitiesHolder.gameObject.activeInHierarchy) AbilityUIManager.Instance.GetOwner().TryUseAbility(currentAbility);
                        ToggleConnectedAbilitiesUI(!additionAbilitiesHolder.gameObject.activeInHierarchy); 
                    }
                    else
                    {
                        PlayerUnit.Instance.TryUseAbility(currentAbility);
                        abilityUIParent.SwapAbility(currentAbility);
                        abilityUIParent.ToggleConnectedAbilitiesUI(false);
                      //  AbilityUIManager.Instance.GlobalCooldown();
                    }
                    break;
                default:  break;
            }
        }
        else
        {
          //  Debug.Log("Ability is not ready");
        }
    }

    public void ActivateNormalCooldown()
    {
        StartCoroutine(CoolDown(currentAbility.abilitySO.abilityAttributie.GetAbilityValueByID("Cooldown").GetValue<float>()));
    }

    public void ActivateGlobalCooldown()
    {
       // if (abilityState == AbilityState.Ready && currentAbility.abilitySO.type != AbilitySO.Type.SetUp)  StartCoroutine(CoolDown(1.5f));
        if (abilityState == AbilityState.Ready) StartCoroutine(CoolDown(1.5f));
    }

    IEnumerator CoolDown(float duration)
    {
        abilityState = AbilityState.OnCooldown;
        coolDownSlider.fillAmount = 1;
        abilityCoolDownText.enabled = true;

        activeTime = duration;

        while (activeTime > 0)
        {
            abilityCoolDownText.text = Utility.FloatToTime(activeTime);
            activeTime -= Time.deltaTime;
            coolDownSlider.fillAmount = activeTime / duration;
            yield return null;
        }
        abilityCoolDownText.enabled = false;
        abilityState = AbilityState.Ready;
    }

    void ResetUI()
    {
        abilityState = AbilityState.Ready;
        coolDownSlider.fillAmount = 0;
        abilityCoolDownText.enabled = false;
    }

    public void SwapAbility(Ability newCurrentAbilitySO)
    {
        connectedAbilityList.Remove(newCurrentAbilitySO);
        connectedAbilityList.Insert(0,currentAbility);
        SetCurrentAbility(newCurrentAbilitySO);
    }

    public void SpawnAdditonalAbilitieUI()
    {
        for (int i = 0; i < connectedAbilityList.Count; i++)
        {
            AbilityHolderUI abilityHolderUI = Instantiate(additionalAbilityPrefab, additionAbilitiesHolder);
            abilityHolderUI.SetCurrentAbility(connectedAbilityList[i]);
            abilityHolderUI.SetAbilityUIParent(this);
            connectedAbilityHolders.Add(abilityHolderUI);
        }
    }

    public void ToggleConnectedAbilitiesUI(bool toggle)
    {
        if (toggle)
        {
            connectedAbilityHolders.Clear();          
            foreach (Transform itemSlot in additionAbilitiesHolder) Destroy(itemSlot.gameObject);
            SpawnAdditonalAbilitieUI();
        }

        additionAbilitiesHolder.gameObject.SetActive(toggle);
    }

    public void Disable()
    {
        if (currentAbility.abilitySO.type == AbilitySO.Type.SetUp || currentAbility.abilitySO.type == AbilitySO.Type.SetUpAndInstantCast) ToggleConnectedAbilitiesUI(false);
    }

    public void ToggleUseAbility(bool toggle)
    {
        GetComponent<Button>().interactable = toggle;
        if (abilityState == AbilityState.OnCooldown) return;
        coolDownSlider.fillAmount = toggle ? 0 : 1;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pos = Input.mousePosition;
        StartCoroutine("TrackTimePressed");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine("TrackTimePressed");
    }

    private IEnumerator TrackTimePressed()
    {
        float time = 0;
        while (time < holdDuration)
        {
            time += Time.deltaTime;
            yield return null;
        }

        buttonHeld = true;

        ToolTipManager.Instance.ShowToolTip(this);
    }

    public void SetAbilityUIParent(AbilityHolderUI abilityHolderUI) => abilityUIParent = abilityHolderUI;

    public string GetHeader() => currentAbility.abilitySO.abilityName;

    public string GetContent() => currentAbility.abilitySO.abilityDescription;

    public Sprite GetBackground() => null;

    public Color GetBackgroundColor() => Color.white;

    public Vector2 GetTocuchPositon() => pos;

    public Action GetAction() => null ;
}
