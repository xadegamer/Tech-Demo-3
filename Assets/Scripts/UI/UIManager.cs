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
    [SerializeField] private CharacterUI targetUI;

    [Header("TargetOfTarget")]
    [SerializeField] private CharacterUI targetOfTargetUI;

    [Header("Buff")]
    [SerializeField] private BuffHolderUI buffHolderUI;

    private void Awake()
    {
        Instance = this;
    } 

    public CharacterUI GetPlayerrUI() => playerUI;
    public CharacterUI GetTargetUI() => targetUI;
    public CharacterUI GetTargetOfTargetUI() => targetOfTargetUI;
}


[Serializable]
public class CharacterUI
{
    [SerializeField] private GameObject holder;
    [SerializeField] private Image icon;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;

    [Header("Buff")]
    [SerializeField] private BuffHolderUI buffHolderUI;
    [SerializeField] private Transform buffHolder;

    private GameUnit gameUnit;

    public void SetUpPlayerUI(GameUnit gameUnit)
    {
        this.gameUnit = gameUnit;
        icon.sprite = gameUnit.GetCharacterClassSO().characterIcon;
        SetHealthBar(gameUnit.HealthHandler.GetNormalisedHealth());
    }

    public void SetUp(GameUnit gameUnit)
    {
        if (this.gameUnit) this.gameUnit.OnHealthChangedEvent -= HealthHandler_OnHealthChanged;
        if (gameUnit)
        {
            this.gameUnit = gameUnit;
            holder.SetActive(true);
            icon.sprite = gameUnit.GetCharacterClassSO().characterIcon;
            gameUnit.OnHealthChangedEvent += HealthHandler_OnHealthChanged;
            SetHealthBar(gameUnit.HealthHandler.GetNormalisedHealth());

            manaBar.gameObject.SetActive(gameUnit is PlayerUnit);

            if (gameUnit is PlayerUnit playerUnit) playerUnit.PlayerStatHandler.GetManaValue().OnValueChanged += StatHandler_OnManaChanged;
        }
        else holder.SetActive(false);
    }

    private void HealthHandler_OnHealthChanged(object sender, float health)
    {
        SetHealthBar(health);
        if (health <= 0) SetUp(null);
    }
    private void StatHandler_OnManaChanged(object sender, float mana)
    {
        SetManaBar(mana);
    }

    public void SetHealthBar(float health) 
    {
        UIManager.Instance.StartCoroutine(Utility.LerpBarValue(healthBar, health, .5f));
      //  healthBar.fillAmount = health; 
    }
    public void SetManaBar(float mana) 
    {
        UIManager.Instance.StartCoroutine(Utility.LerpBarValue(manaBar, mana, .5f));
       // manaBar.fillAmount = mana; 
    }

    public void AddBuff(Buff buff)
    {
        GameObject.Instantiate(buffHolderUI, buffHolder).ActivateBuff(buff);
    }

    public void RemoveBuff(BuffSO buff)
    {

    }

    public void ClearBuffs()
    {

    }

    public void Hide()
    {
        holder.SetActive(false);
    }
}