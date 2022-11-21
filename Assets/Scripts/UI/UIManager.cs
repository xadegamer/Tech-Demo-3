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

    public void SetUp(CharacterClassSO characterClass)
    {
        if(characterClass)
        {
            holder.SetActive(true);
            icon.sprite = characterClass.characterIcon;
        } else holder.SetActive(false);
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