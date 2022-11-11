using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.VisualScripting;

public class AbilityHolderUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum AbilityState
    {
        Ready,
        OnCooldown,      
    }

    [Header("Hold Option")]
    [Range(1.0f, 10.0f)]
    [SerializeField] public float holdDuration = 1.0f;
    
    [Header("Ability Option")]
    [SerializeField] private AbilityState abilityState;
    [SerializeField] private AbilitySO abilitySO;
    [SerializeField] private Image abilityImage;
    [SerializeField] private Image coolDownSlider;
    [SerializeField] private TextMeshProUGUI abilityCoolDownText;

    [Header("Additional Ability Option")]
    [SerializeField] private AbilityHolderUI additionalAbilityPrefab;
    [SerializeField] private Transform additionAbilitiesHolder;
    [SerializeField] private List<AbilityHolderUI> additionalAbilityHolders;
    
    private AbilityHolderUI abilityUIParent;
    private float activeTime;

    private void Start()
    {
        ResetUI();
    }

    public void SetAbility(AbilitySO abilitySO)
    {
        this.abilitySO = abilitySO;
        abilityImage.sprite = abilitySO.abilityIcon;
    }

    public void Use()
    {

        if (abilityState == AbilityState.Ready)
        {
            switch (abilitySO.type)
            {
                case AbilitySO.Type.InstantCast:
                    abilitySO.UseAbility();
                    StartCoroutine(CoolDown());
                    break;
                case AbilitySO.Type.DelayedCast:
                    abilitySO.UseAbility();
                    break;
                case AbilitySO.Type.SetUp:

                    if (!abilityUIParent)
                    {
                        ToggleAdditionalAbilitiesUI(!additionAbilitiesHolder.gameObject.activeInHierarchy);
                    }
                    else
                    {
                        Debug.Log("New Ability Clicked");
                        abilitySO.UseAbility();
                        abilityUIParent.SetAbility(abilitySO);
                        abilityUIParent.ToggleAdditionalAbilitiesUI(false);
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

        float cooldownTime = abilitySO.abilityData.GetAbilityValueByID("Cooldown").GetValue();
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

    public void SpawnAdditonalAbilitieUI()
    {
        for (int i = 0; i < abilitySO.optionalabilities.Length; i++)
        {
            AbilityHolderUI abilityHolderUI = Instantiate(additionalAbilityPrefab, additionAbilitiesHolder);
            abilityHolderUI.SetAbility(abilitySO.optionalabilities[i]);
            abilityHolderUI.SetAbilityUIParent(this);
            additionalAbilityHolders.Add(abilityHolderUI);
        }
    }

    public void ToggleAdditionalAbilitiesUI(bool toggle)
    {
        if (toggle)
        {
            additionalAbilityHolders.Clear();          
            foreach (Transform itemSlot in additionAbilitiesHolder) Destroy(itemSlot.gameObject);
            SpawnAdditonalAbilitieUI();
        }

        additionAbilitiesHolder.gameObject.SetActive(toggle);
    }

    public void Disable()
    {
        if (abilitySO.type == AbilitySO.Type.SetUp) ToggleAdditionalAbilitiesUI(false);
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

        ToggleAdditionalAbilitiesUI(true);
        Debug.Log("Held Down");
    }

    public void SetAbilityUIParent(AbilityHolderUI abilityHolderUI)
    {
        abilityUIParent = abilityHolderUI;
    }
}
