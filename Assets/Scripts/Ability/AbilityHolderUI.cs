using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using System.Linq;

public class AbilityHolderUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum AbilityState
    {
        Ready,
        OnCooldown,      
    }

    [Header("Ability UI")]
    [SerializeField] private Image abilityImage;
    [SerializeField] private Image coolDownSlider;
    [SerializeField] private TextMeshProUGUI abilityCoolDownText;

    [Header("Hold Option")]
    [Range(1.0f, 10.0f)]
    [SerializeField] public float holdDuration = 1.0f;

    [Header("Ability Option")]
    [SerializeField] private AbilityState abilityState;
    [SerializeField] private AbilitySO currentAbilitySO;
    [SerializeField] private List<AbilitySO> connectedAbilitySOList;

    [Header("Additional Ability Option")]
    [SerializeField] private AbilityHolderUI additionalAbilityPrefab;
    [SerializeField] private Transform additionAbilitiesHolder;
    [SerializeField] private List<AbilityHolderUI> connectedAbilityHolders;
    
    private AbilityHolderUI abilityUIParent;
    private float activeTime;

    private void Start()
    {
        ResetUI();
    }

    public void SetCurrentAbility(AbilitySO abilitySO)
    {
        abilityImage.sprite = abilitySO.abilityIcon;
        currentAbilitySO = abilitySO;
    }

    public void SetConnectedAbilities(AbilitySO[] abilitySOs = null)
    {
        abilitySOs?.ToList().ForEach(x => connectedAbilitySOList.Add(x));
    }

    public void Use()
    {
        if (abilityState == AbilityState.Ready)
        {
            switch (currentAbilitySO.type)
            {
                case AbilitySO.Type.InstantCast:
                    currentAbilitySO.UseAbility();
                    StartCoroutine(CoolDown());
                    break;
                case AbilitySO.Type.DelayedCast:
                    currentAbilitySO.UseAbility();
                    break;
                case AbilitySO.Type.SetUp:

                    if (!abilityUIParent)
                    {
                        ToggleConnectedAbilitiesUI(!additionAbilitiesHolder.gameObject.activeInHierarchy);
                    }
                    else
                    {
                        Debug.Log("New Ability Clicked");
                        currentAbilitySO.UseAbility();
                        abilityUIParent.SwapAbility(currentAbilitySO);
                        abilityUIParent.ToggleConnectedAbilitiesUI(false);
                    }
                    
                    break;
                default:  break;
            }
        }
        else
        {
            Debug.Log("Ability is not ready");
        }
    }
    
    IEnumerator CoolDown()
    {
        abilityState = AbilityState.OnCooldown;
        coolDownSlider.fillAmount = 1;
        abilityCoolDownText.enabled = true;

        float cooldownTime = currentAbilitySO.abilityData.GetAbilityValueByID("Cooldown").GetValue();
        activeTime = cooldownTime;

        while (activeTime > 0)
        {
            DisplayTime(activeTime);
            activeTime -= Time.deltaTime;
            coolDownSlider.fillAmount = activeTime / cooldownTime;
            yield return null;
        }
        abilityCoolDownText.enabled = false;
        abilityState = AbilityState.Ready;
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0) return;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = timeToDisplay % 60;
   
        abilityCoolDownText.text = (minutes == 0) ? $"{seconds.ToString("F1")}s" : $"{minutes:00}m : {seconds:00}s";
    }

    void ResetUI()
    {
        abilityState = AbilityState.Ready;
        coolDownSlider.fillAmount = 0;
        abilityCoolDownText.enabled = false;
    }

    public void SwapAbility(AbilitySO newCurrentAbilitySO)
    {
        connectedAbilitySOList.Remove(newCurrentAbilitySO);
        connectedAbilitySOList.Insert(0,currentAbilitySO);
        SetCurrentAbility(newCurrentAbilitySO);
    }

    public void SpawnAdditonalAbilitieUI()
    {
        for (int i = 0; i < connectedAbilitySOList.Count; i++)
        {
            AbilityHolderUI abilityHolderUI = Instantiate(additionalAbilityPrefab, additionAbilitiesHolder);
            abilityHolderUI.SetCurrentAbility(connectedAbilitySOList[i]);
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
        if (currentAbilitySO.type == AbilitySO.Type.SetUp) ToggleConnectedAbilitiesUI(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       // StartCoroutine(TrackTimePressed());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
    }

    private IEnumerator TrackTimePressed()
    {
        float time = 0;
        while (time < holdDuration)
        {
            time += Time.deltaTime;
            yield return null;
        }

        ToggleConnectedAbilitiesUI(true);
        Debug.Log("Held Down");
    }

    public void SetAbilityUIParent(AbilityHolderUI abilityHolderUI)
    {
        abilityUIParent = abilityHolderUI;
    }
}
