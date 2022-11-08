using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AbilityHolderUI : MonoBehaviour
{
    public enum AbilityState
    {
        Ready,
        OnCooldown,      
    }

    [SerializeField] private AbilityState abilityState;
    [SerializeField] private AbilitySO abilitySO;
    [SerializeField] private Image abilityImage;
    [SerializeField] private Image coolDownSlider;
    [SerializeField] private TextMeshProUGUI abilityCoolDownText;

    private float activeTime;

    private void Start()
    {
        ResetUI();
    }

    public void SetAbility(AbilitySO abilitySO)
    {
        this.abilitySO = abilitySO;
    }

    public void Use()
    {
        if (abilityState == AbilityState.Ready)
        {
            abilitySO.UseAbility();
            if (abilitySO.abilityData.castTime == 0) StartCoroutine(CoolDown());
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
        activeTime = abilitySO.abilityData.coolDownTime;
        
        while (activeTime > 0)
        {
            activeTime -= Time.deltaTime;
            coolDownSlider.fillAmount = activeTime / abilitySO.abilityData.coolDownTime;
            DisplayTime(activeTime);
            yield return null;
        }
        abilityCoolDownText.enabled = false;
        abilityState = AbilityState.Ready;
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0) return;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        abilityCoolDownText.text = (minutes == 0) ?string.Format("{0}s", seconds)  : string.Format("{0:00}s:{1:00}m", minutes, seconds); ;
    }

    void ResetUI()
    {
        abilityState = AbilityState.Ready;
        coolDownSlider.fillAmount = 0;
        abilityCoolDownText.enabled = false;
    }
}
