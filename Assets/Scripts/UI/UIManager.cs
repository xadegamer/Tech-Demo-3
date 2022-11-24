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

    [Header("Attacking")]
    [SerializeField] private GameObject attackButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerUnit.Instance.OnTargetFound += ToggleAttackButton;
    }

    public CharacterUI GetPlayerrUI() => playerUI;
    public CharacterUI GetTargetUI() => targetUI;
    public CharacterUI GetTargetOfTargetUI() => targetOfTargetUI;

    public void ToggleAttackButton(bool toggle)
    {
        attackButton.SetActive(toggle);
    }
}

[Serializable]
public class CharacterUI
{
    [SerializeField] private GameObject holder;
    [SerializeField] private Image icon;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;

    [Header("Buff")]
    [SerializeField] private BuffObjectUI buffObjectUI;
    [SerializeField] private Transform buffHolder;

    private GameUnit gameUnit;

    public void SetUpPlayerUI(GameUnit gameUnit)
    {
        this.gameUnit = gameUnit;
        icon.sprite = gameUnit.GetCharacterClassSO().characterIcon;
        SetHealthBar(gameUnit.HealthHandler.GetNormalisedHealth());
        SpawnBuff();
        gameUnit.gameUnitBuffController.OnBuffAdded += AddBuffObject;
    }

    public void SetUp(GameUnit gameUnit)
    {
        if (this.gameUnit != null)
        {
            this.gameUnit.OnHealthChangedEvent -= HealthHandler_OnHealthChanged;
            if (buffHolder) this.gameUnit.gameUnitBuffController.OnBuffAdded -= AddBuffObject;
            this.gameUnit = null;
        }
        
        if (buffHolder) ClearBuffObjects();
        
        if (gameUnit)
        {
            this.gameUnit = gameUnit;
            
            if(buffHolder) SpawnBuff();
            
            holder.SetActive(true);
            icon.sprite = gameUnit.GetCharacterClassSO().characterIcon;
            gameUnit.OnHealthChangedEvent += HealthHandler_OnHealthChanged;
            SetHealthBar(gameUnit.HealthHandler.GetNormalisedHealth());

            if (buffHolder) gameUnit.gameUnitBuffController.OnBuffAdded += AddBuffObject;

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
        UIManager.Instance.StartCoroutine(Utility.LerpBarValue(healthBar, health, .2f));
    }
    public void SetManaBar(float mana) 
    {
        UIManager.Instance.StartCoroutine(Utility.LerpBarValue(manaBar, mana, .2f));
    }

    public void AddBuffObject(BuffObject buffObject)
    {
        GameObject.Instantiate(buffObjectUI, buffHolder).SetUp(buffObject);
    }

    public void SpawnBuff()
    {
        foreach (BuffObject buffObject in gameUnit.gameUnitBuffController.GetBuffObjects())
        {
            AddBuffObject(buffObject);
        }
    }

    public void ClearBuffObjects()
    {
        buffHolder.DestroyAllChildren();
    }

    public void Hide()
    {
        holder.SetActive(false);
    }
}