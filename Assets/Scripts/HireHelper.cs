using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;


enum HireHelperType
{
    Waitress,
    HeadWaitress
}

public class HireHelper : MonoBehaviour
{

    [Header("=== Identifier ===")]
    public string ID;
    public string StationName;


    public int helperCap = 5;
    public float EachLevelMoney = 2000;
    public float multiply = 5;
    public TextMeshProUGUI countText;
    [SerializeField] GameObject LockedFiller;
    [SerializeField] GameObject FullCap;
    [SerializeField] List<GameObject> helpersPrefab;
    [SerializeField] List<HelperController> helpers;
    [SerializeField] List<HeadHelperController> headHelpers;
    [SerializeField] Transform helperParent;
    [SerializeField] Transform startPos;
    [SerializeField] Transform waitingPos;
    [SerializeField] Transform CollectPos;
    [SerializeField] Transform sink;
    [SerializeField] ConveyorController conveyor;
    public List<MarketOpener> tablePos;
    [SerializeField] HireHelperType currType;

    int SpeedLevel =1;
    int CollectSpeedLevel=1;
    int CapacityLevel=1;

    [HideInInspector] public float TotalMoney;
    bool canGiveMoney;
    [HideInInspector] public float investedPrice;
    [SerializeField] Image MoneyFiller;
    [SerializeField] TextMeshProUGUI MoneyText;
    void Start()
    {
        if (currType == HireHelperType.Waitress)
            TotalMoney = EachLevelMoney * multiply;
        if (currType == HireHelperType.HeadWaitress)
            TotalMoney = EachLevelMoney * multiply;

        LoadData();
        refreshMoney();
    }


    private void LoadData()
    {
        var helpersCount = PlayerPrefs.GetInt(ID + StationName + "helpersCount", 0);
        for (int i = 0; i < helpersCount; i++) HireEmployee(true);
        investedPrice = PlayerPrefs.GetFloat(ID + StationName + "investedPrice", 0);
        var headhelpersCount = PlayerPrefs.GetInt(ID + StationName + "headhelpersCount", 0);
        for (int i = 0; i < headhelpersCount; i++) HireEmployee(true);
    }

    private void SaveData()
    {
        PlayerPrefs.SetFloat(ID + StationName + "investedPrice", investedPrice);
        PlayerPrefs.SetInt(ID + StationName + "helpersCount", helpers.Count);
        PlayerPrefs.SetInt(ID + StationName + "headhelpersCount", headHelpers.Count);

    }

    void refreshMoney()
    {
        if(currType == HireHelperType.Waitress)
        {
        
        countText.text = helpers.Count.ToString() + "/" + helperCap.ToString();

        if (helpers.Count == helperCap)
        {
            GetComponent<BoxCollider>().enabled = false;
            LockedFiller.SetActive(false);
            FullCap.SetActive(true);
        }

        MoneyFiller.fillAmount = investedPrice / TotalMoney;
        MoneyText.text = (TotalMoney - investedPrice).ToString();
        SaveData();
        }
        else if (currType == HireHelperType.HeadWaitress)
        {
            countText.text = headHelpers.Count.ToString() + "/" + helperCap.ToString();

            if (headHelpers.Count == helperCap)
            {
                GetComponent<BoxCollider>().enabled = false;
                LockedFiller.SetActive(false);
                FullCap.SetActive(true);
            }

            MoneyFiller.fillAmount = investedPrice / TotalMoney;
            MoneyText.text = (TotalMoney - investedPrice).ToString();
            SaveData();

        }
    }

    public float Payment(float givenPrice)
    {
        if (canGiveMoney)
        {
            if (TotalMoney - investedPrice > 0)
            {
                investedPrice += givenPrice;
                refreshMoney();
                if (TotalMoney - investedPrice <= 0)
                    HireEmployee(false);

                return -givenPrice;
            }
            else
            {
                HireEmployee(false);
                return -(TotalMoney - investedPrice);
            }
        } 
        else return 0;
    }
    void HireEmployee(bool isLoad)
    {
        if (currType == HireHelperType.Waitress)
        {
        var helper = helpersPrefab[Random.Range(0, helpersPrefab.Count)];
        var x = Instantiate(helper, helperParent);
        x.GetComponent<NavMeshAgent>().enabled = false;
        x.transform.position = startPos.position;
        x.transform.parent = helperParent;
        var helperConroller = x.GetComponent<HelperController>();
        helperConroller.tablePos = tablePos;
        helperConroller.Collect = CollectPos;
        helperConroller.WaitPos = waitingPos;
        helperConroller.sink = sink;
        helperConroller.conveyor = conveyor;
        helperConroller.WaitPos = waitingPos;        
        helperConroller.setSpeedLevel(SpeedLevel);
        helperConroller.setCapacityLevel(CapacityLevel);
        helperConroller.setCollectSpeedLevel(CollectSpeedLevel);
        helpers.Add(helperConroller);
        investedPrice = 0;
        TotalMoney = EachLevelMoney * multiply * (helpers.Count + 1);
        refreshMoney();
        canGiveMoney = false;
        if (!isLoad) GameSingleton.Instance.Sounds.PlayOneShot(GameSingleton.Instance.Sounds.OpenStation);
    }
        else if (currType == HireHelperType.HeadWaitress)
        {
            var headhelper = helpersPrefab[Random.Range(0, helpersPrefab.Count)];
            var x = Instantiate(headhelper, helperParent);
            headhelper.GetComponent<NavMeshAgent>().enabled = false;
            x.transform.position = startPos.position;
            x.transform.parent = helperParent;
            var headhelperController = x.GetComponent<HeadHelperController>();
            headhelperController.orderPos = tablePos;
            headhelperController.WaitPos = waitingPos;
            headHelpers.Add(headhelperController); 
            investedPrice = 0;
            TotalMoney = EachLevelMoney * multiply * (headHelpers.Count + 1);
            refreshMoney();
            canGiveMoney = false;
            if (!isLoad) GameSingleton.Instance.Sounds.PlayOneShot(GameSingleton.Instance.Sounds.OpenStation);

        }
    }
    
    public void setHelperSpeedLevel(int currLevel)
    {
        SpeedLevel = currLevel;
        foreach (var helper in helpers)
            helper.setSpeedLevel(SpeedLevel);
    }

    internal void setHelperCollectSpeedLevel(int currLevel)
    {
        CollectSpeedLevel = currLevel;
        foreach (var helper in helpers)
            helper.setCollectSpeedLevel(CollectSpeedLevel);
    }

    internal void setHelperCapacityLevel(int currLevel)
    {
        CapacityLevel = currLevel;
        foreach (var helper in helpers)
            helper.setCapacityLevel(CapacityLevel);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            canGiveMoney = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canGiveMoney = true;
        }
    }
}
