using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaOpener : MonoBehaviour
{
    [Header("=== Identifier ===")]
    public string ID;
    public string AreaName;


    [SerializeField] GameObject player;
    [SerializeField] GameObject openArea;
    [SerializeField] GameObject unlockArea;

    [SerializeField] List<GameObject> hideObjectsAfterOpen;
    [SerializeField] List<GameObject> showObjectsAfterOpen;

    public float TotalMoney;
    [HideInInspector] public float investedPrice;
    [SerializeField] Image MoneyFiller;
    [SerializeField] TextMeshProUGUI MoneyText;
    [HideInInspector] public bool isOpen;
     public bool CloseInStart;

    private void Awake()
    {
        if (CloseInStart)
            gameObject.SetActive(false);
    }
    void Start()
    {
        showObjectsAfterOpen.ForEach(x => x.SetActive(false));
        LoadData();
        refreshMoney();
        Payment(0);

    }

    private void LoadData()
    {
        investedPrice = PlayerPrefs.GetFloat(ID + AreaName + "investedPrice", 0);
    }

    private void SaveData()
    {
        PlayerPrefs.SetFloat(ID + AreaName + "investedPrice", investedPrice);
    }
    void refreshMoney()
    {
        MoneyFiller.fillAmount = investedPrice / TotalMoney;
        MoneyText.text = (TotalMoney - investedPrice).ToString();
        SaveData();
    }

    public float Payment(float givenPrice)
    {
        if (TotalMoney - investedPrice > 0)
        {
            investedPrice += givenPrice;
            refreshMoney();
            if (TotalMoney - investedPrice <= 0)
            {
                GameSingleton.Instance.Sounds.PlayOneShot(GameSingleton.Instance.Sounds.OpenMarket);
                openArea.SetActive(true);
                unlockArea.SetActive(false);
                foreach (var item in hideObjectsAfterOpen)
                    item.SetActive(false);

                foreach (var item in showObjectsAfterOpen)
                    item.SetActive(true);

                isOpen = true;
            }

            return -givenPrice;
        }
        else
        {
            openArea.SetActive(true);
            unlockArea.SetActive(false);
            isOpen = true;
            foreach (var item in hideObjectsAfterOpen)
                item.SetActive(false);

            foreach (var item in showObjectsAfterOpen)
                item.SetActive(true);

            return -(TotalMoney - investedPrice);
        }
    }
}
