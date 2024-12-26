using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MoneyAreaController : MonoBehaviour
{
    public int moneyCap;
    [HideInInspector] public int moneyAmount;
    [HideInInspector] public List<GameObject> moneys;
    [SerializeField] Transform moneyPos;
    [Range(0,5)] [SerializeField] float ObjectSpacingHeight;
    [SerializeField] GameObject objectPrefab;
    [SerializeField] StationOpener marketStationOpener;
    [SerializeField] GameObject moneyMax;
    [SerializeField] TextMeshProUGUI moneyAmountText;

    private void Awake()
    {
        LoadData();
    }

    private void LoadData()
    {
        moneyTextWriter();
        var moneysCount = PlayerPrefs.GetInt(marketStationOpener.ID + "moneysCount", 0);
        if (moneysCount > 0)
            addMoney(moneysCount);
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(marketStationOpener.ID + "moneysCount", moneyAmount);
    }

    public void addMoney(int pm)
    {
        if (moneyAmount >= moneyCap)
            return;
        else if (moneyAmount + pm > moneyCap)
            moneyAmount = moneyCap;
        else
            moneyAmount += pm;

        var x = Instantiate(objectPrefab, moneyPos);
        x.transform.parent = moneyPos;
        x.transform.position += transform.up * moneys.Count * ObjectSpacingHeight;
        moneys.Add(x);
        moneyTextWriter();
        SaveData();
    }

    void moneyTextWriter()
    {
        if (moneyAmount == moneyCap)
        {
            moneyMax.SetActive(true);
            moneyAmountText.gameObject.SetActive(false);
        }
        else
        {
            moneyAmountText.text = moneyAmount + "/" + moneyCap;
            moneyAmountText.gameObject.SetActive(true);
            moneyMax.SetActive(false);
        }
    }
    public void removeMoney(int mm)
    {
        var x = moneys.Last();
        moneys.Remove(x);
        Destroy(x);
        x = null;
        moneyAmount -= mm;
        moneyTextWriter();
        SaveData();
    }
}
