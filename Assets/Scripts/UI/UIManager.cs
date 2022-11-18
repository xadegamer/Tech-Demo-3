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

    public void SpawnPlayerBuffUI(Buff buff)
    {
        playerUI.AddBuff(buffHolderUI,buff);
    }
}


[Serializable]
public class CharacterUI
{
    [SerializeField] private GameObject holder;
    [SerializeField] private Image icon;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private Transform buffHolder;

    public void SetUp(Sprite sprite)
    {
        icon.sprite = sprite;
        holder.SetActive(true);
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

    public void AddBuff(BuffHolderUI buffHolderUI , Buff buff)
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