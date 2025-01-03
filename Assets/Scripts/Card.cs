using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Card :MonoBehaviour
{
    public bool isLoaded;
    [SerializeField] string cardName;
    public int currLevel;
    [SerializeField] List<GameObject> levels;
    [SerializeField] float startMoney;
     float currMoney;
    [SerializeField] int multipleMoney;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] string maxLevelText;
    [SerializeField] Button AdButton;
    [SerializeField] Button MaxButton;

    private void Awake()
    {
        AdButton.interactable = false;
        LoadData();
        RefreshUI();
    }

    private void RefreshUI()
    {
        for (int i = 0; i < currLevel; i++) levels[i].SetActive(true);
        currMoney = startMoney * currLevel * multipleMoney;
        moneyText.text =  LunesHelper.CrunchNumbers(startMoney * currLevel * multipleMoney).ToString();
        if (currLevel == levels.Count) moneyText.text = maxLevelText;

        if (currLevel == 1 || levels.Count == currLevel)
        {
            AdButton.interactable = false;
            MaxButton.interactable = false;
        }
        else
        {
            AdButton.interactable = true;
            MaxButton.interactable = true;
        }
        if (currLevel == 1)
            MaxButton.interactable = true;


        SaveData();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(cardName + "currLevel",currLevel);
    }

    public void LoadData()
    {
        if(!isLoaded)
        {
            isLoaded = true;
            var currLevel = PlayerPrefs.GetInt(cardName + "currLevel", 1);
            for (int i = 1; i < currLevel; i++)
                LevelUpgradeForLoad();
            this.currLevel = currLevel;
            RefreshUI();
        }
    }

    public void LevelUpgrade()
    {
        if (currLevel != levels.Count && GameSingleton.Instance.getMoney() >= currMoney)
        {
            GameSingleton.Instance.SetMoney(-currMoney);
            currLevel++;
            RefreshUI();
        }

    }
    public void LevelUpgradeForLoad()
    {
        if(currLevel != levels.Count)
        {
            currLevel++;
            RefreshUI();
        }
    }
}

