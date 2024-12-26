using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLogic : MonoBehaviour
{
    [Header("=== Player Identifier ===")]
    public string ID;
    public string Name;

    [SerializeField] float UnlockStationTimer;
    [SerializeField] FoodHolder foodHolder;
    int carryBoxObjectLimit;
    int currCarryBoxObject;
    Animator animator;
    AudioSource audioSource;
    [SerializeField] JoystickLogic movment;
    public MoneyController moneyMoverForUI;
    bool canIntract;

    [SerializeField] GameObject waitressPlayer;
    [SerializeField] GameObject cookPlayer;
    [SerializeField] GameObject dirtyPlate;


    [Header("=== Timer Logic ===")]

    [Range(0, 5f)][SerializeField] float collectElapsed;
    [Range(0, 5f)][SerializeField] float purchaseElapsed;
    [Range(0, 5f)][SerializeField] float refillElapsed;
    [Range(0, 5f)][SerializeField] float moneyElapsed;
    float refillTimer;
    float collectTimer;
    float purchaseTimer;
    float moneyTimer;


    [Header("== Upgrade Logic ==")]
    [SerializeField] List<float> speedLevelAmount;
    [SerializeField] List<float> collectSpeedLevelAmount;
    [SerializeField] List<int> capacityLevelAmount;
    int collectSpeedLevel=1;
    int speedLevel = 1;
    int capacityLevel = 1;

    private bool playerStartGathering;
    List<OrderDTO> currFood = new List<OrderDTO>() ;
    private bool insideKitchen;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        LoadData();
    }

    void Update()
    {
        AnimationControll(); SaveData();
    }
    private void LoadData()
    {
        var x = PlayerPrefs.GetFloat(ID + Name + "PosX", 41);
        var y = PlayerPrefs.GetFloat(ID + Name + "PosY", 0.15f);
        var z = PlayerPrefs.GetFloat(ID + Name + "PosZ", 13);
        transform.position = new Vector3(x, y, z);

       /* var DirtyDishCount = PlayerPrefs.GetInt(ID + Name + "CarryFoodType.DirtyDish", foodHolder.countTray(CarryFoodType.DirtyDish));
        var SteakCount = PlayerPrefs.GetInt(ID + Name + "CarryFoodType.Steak", foodHolder.countTray(CarryFoodType.Steak));
        var FastFoodCount = PlayerPrefs.GetInt(ID + Name + "CarryFoodType.FastFood", foodHolder.countTray(CarryFoodType.FastFood));
        var IceCreamCount = PlayerPrefs.GetInt(ID + Name + "CarryFoodType.IceCream", foodHolder.countTray(CarryFoodType.IceCream));
        var PizzaCount = PlayerPrefs.GetInt(ID + Name + "CarryFoodType.Pizza", foodHolder.countTray(CarryFoodType.Pizza));

        for (int i = 0; i < DirtyDishCount; i++)
            foodHolder.addObject(CarryFoodType.DirtyDish);

        for (int i = 0; i < SteakCount; i++)
            foodHolder.addObject(CarryFoodType.Steak);

        for (int i = 0; i < FastFoodCount; i++)
            foodHolder.addObject(CarryFoodType.FastFood);

        for (int i = 0; i < IceCreamCount; i++)
            foodHolder.addObject(CarryFoodType.IceCream);

        for (int i = 0; i < PizzaCount; i++)
            foodHolder.addObject(CarryFoodType.Pizza);
       */
    }

    private void SaveData()
    {
        PlayerPrefs.SetFloat(ID + Name + "PosX", transform.position.x);
        PlayerPrefs.SetFloat(ID + Name + "PosY", transform.position.y);
        PlayerPrefs.SetFloat(ID + Name + "PosZ", transform.position.z);
        //SaveCarryData();
    }

    private void SaveCarryData()
    {
        PlayerPrefs.SetInt(ID + Name + "CarryFoodType.DirtyDish", foodHolder.countTray(CarryFoodType.DirtyDish));
        PlayerPrefs.SetInt(ID + Name + "CarryFoodType.Steak", foodHolder.countTray(CarryFoodType.Steak));
        PlayerPrefs.SetInt(ID + Name + "CarryFoodType.FastFood", foodHolder.countTray(CarryFoodType.FastFood));
        PlayerPrefs.SetInt(ID + Name + "CarryFoodType.IceCream", foodHolder.countTray(CarryFoodType.IceCream));
        PlayerPrefs.SetInt(ID + Name + "CarryFoodType.Pizza", foodHolder.countTray(CarryFoodType.Pizza));
    }

    private void AnimationControll()
    {
        //animator.SetBool("Carry", (carryObjects.carryObjets.Count > 0 || carryBox.carryBoxs.Count > 0) && !currSprout.isStartGethering);
        canIntract = animator.GetFloat("speed") == 0;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == Constant.Tags.UpgradeHelper) GameSingleton.Instance.UI.UpgradeHelper.SetActive(false);
        if(other.tag == Constant.Tags.UpgradePlayer) GameSingleton.Instance.UI.UpgradePlayer.SetActive(false);
        if(other.tag == Constant.Tags.Food)
        {
            refillTimer = 0;
            var foodConveyorController = other.GetComponent<FoodConveyorController>();
            foodConveyorController.inTimer = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == Constant.Tags.Refill && foodHolder.carryObjects.Contains(CarryFoodType.DirtyDish))
        {
            refillTimer += Time.deltaTime;
            if (refillTimer > refillElapsed)
            {
                refillTimer = 0;
                var carryStation = other.GetComponent<RefillArea>();
                if (foodHolder.carryObjects.Contains(CarryFoodType.DirtyDish))
                {
                    var pos = foodHolder.removeObject(CarryFoodType.DirtyDish);
                    var x = Instantiate(dirtyPlate, null);
                    x.transform.position = pos;
                    carryStation.goToTrash(x,true);
                }
            }
        }

        if (other.gameObject.tag == Constant.Tags.MoneyArea && other.gameObject.transform.parent.GetComponent<MoneyAreaController>().moneys.Count > 0)
        {
            var moneyAreaController = other.gameObject.transform.parent.GetComponent<MoneyAreaController>();
            if (moneyAreaController.moneyAmount > 0)
            {
                moneyTimer += Time.deltaTime;
                if (moneyTimer > moneyElapsed)
                {
                    moneyTimer = 0;
                    var addingMoney = moneyAreaController.moneyAmount / moneyAreaController.moneys.Count;
                    moneyMoverForUI.StartCoinMove(moneyAreaController.moneys.Last().transform.position, addingMoney);
                    moneyAreaController.removeMoney(addingMoney);
                }
            }
        }
        if (other.gameObject.tag == Constant.Tags.Coin)
        {
            moneyMoverForUI.StartCoinMove(other.transform.position, other.gameObject.GetComponent<moneyCoin>().priceValue);
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == Constant.Tags.Food && canIntract)
        {
            var foodConveyorController = other.GetComponent<FoodConveyorController>();
            if (foodHolder.carryObjects.Count < foodHolder.carryLimit)
            {
                refillTimer += Time.deltaTime;
                if (refillTimer > refillElapsed)
                {
                    refillTimer = 0;
                    foodHolder.addObject(foodConveyorController.orderDTO.foodType);
                    GameSingleton.Instance.Sounds.PlayOneShot(GameSingleton.Instance.Sounds.Take);
                    foodConveyorController.Remove();
                }
                else
                {
                    foodConveyorController.SetTimer(Time.deltaTime, refillElapsed);
                    foodConveyorController.inTimer = true;
                }
            }
        }
        if (other.gameObject.tag == Constant.Tags.forOrder)
        {
            var markett = other.GetComponent<MarketOpener>();
            if (markett.currCustomer != null || markett.currVIPCustomer != null)
            {
                if (markett.currCustomer != null)
                {
                    if (markett.currCustomer.handUp)
                        markett.currCustomer.handUp = false;
                    else if (markett.currCustomer.wantFood && foodHolder.carryObjects.Contains(markett.currCustomer.wantFoodType))
                    {
                        markett.currCustomer.wantFood = false;
                        markett.addFood();
                        foodHolder.removeObject(markett.currCustomer.wantFoodType);
                        GameSingleton.Instance.Sounds.PlayOneShot(GameSingleton.Instance.Sounds.Drop);
                    }
                }
                else if (markett.currVIPCustomer != null)
                {
                    if (markett.currVIPCustomer.handUp)
                        markett.currVIPCustomer.handUp = false;
                    else if (markett.currVIPCustomer.wantFood)
                    {
                        List<CarryFoodType> deletedList = new List<CarryFoodType>();
                        foreach(var x in markett.currVIPCustomer.wantFoodType)
                        {
                           if(foodHolder.carryObjects.Contains(x))
                            {
                                deletedList.Add(x);
                                markett.addVIPFood(x);
                                foodHolder.removeObject(x);
                            }
                        }
                        foreach (var x in deletedList)
                        {
                            markett.currVIPCustomer.wantFoodType.Remove(x);
                        }
                        if(markett.currVIPCustomer.wantFoodType.Count == 0)
                            markett.currVIPCustomer.wantFood = false;
                    }
                }
               
            }
            else
            {
                if (markett.dirtyDish.activeInHierarchy && foodHolder.carryObjects.Count < foodHolder.carryLimit)
                {
                    markett.clearTable();
                    foodHolder.addObject(CarryFoodType.DirtyDish);
                    GameSingleton.Instance.Sounds.PlayOneShot(GameSingleton.Instance.Sounds.Drop);
                }
            }

        }
        
        if (other.gameObject.tag == Constant.Tags.OpenStation && canIntract)
        {
            moneyElapsed += Time.deltaTime;
            if (moneyTimer < moneyElapsed)
            {
                moneyElapsed = 0;
                var stationOpener = other.GetComponent<StationOpener>();
                var money = CalculateGivenMoney(stationOpener.TotalMoney, (stationOpener.TotalMoney - stationOpener.investedPrice));
                if (GameSingleton.Instance.Money >= money && money != 0)
                {
                    GameSingleton.Instance.SetMoney(stationOpener.Payment(money));
                }
            }
        }
        if (other.gameObject.tag == Constant.Tags.FreeOpenStation && canIntract)
        {
            var stationOpener = other.GetComponent<StationOpener>();
            refillTimer += Time.deltaTime;
            if (refillTimer > refillElapsed && !stationOpener.isOpen)
            {
                refillTimer = 0;
                other.GetComponent<rewardedTable>().UserChoseToWatchAd();
            }
            else
            {
                stationOpener.SetTimer(Time.deltaTime, refillElapsed);
                stationOpener.inTimer = true;
            }
        }
        if (other.gameObject.tag == Constant.Tags.HireHelper && canIntract)
        {
            var hireHelper = other.GetComponent<HireHelper>();
            var money = CalculateGivenMoney(hireHelper.TotalMoney, (hireHelper.TotalMoney - hireHelper.investedPrice));
            if (GameSingleton.Instance.Money >= money && money != 0)
            {
                GameSingleton.Instance.SetMoney(hireHelper.Payment(money));
            }

        }

        if (other.gameObject.tag == Constant.Tags.OpenLand && canIntract)
        {
            moneyElapsed += Time.deltaTime;
            if (moneyTimer < moneyElapsed)
            {
                moneyElapsed = 0;
                var areaOpener = other.GetComponent<AreaOpener>();
                var money = CalculateGivenMoney(areaOpener.TotalMoney, (areaOpener.TotalMoney - areaOpener.investedPrice));
                if (GameSingleton.Instance.Money >= money && money != 0)
                {
                    GameSingleton.Instance.SetMoney(areaOpener.Payment(money));
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Constant.Tags.UpgradeHelper) GameSingleton.Instance.UI.UpgradeHelper.SetActive(true);
        if (other.gameObject.tag == Constant.Tags.UpgradePlayer) GameSingleton.Instance.UI.UpgradePlayer.SetActive(true);
        if (other.gameObject.tag == Constant.Tags.Food)
        {
            var foodConveyorController = other.GetComponent<FoodConveyorController>();
            refillTimer = foodConveyorController.currTimer;
        }
        // Teleport
        if (other.gameObject.tag == "ChangeCostume")
        {
            var changer = other.GetComponent<kitchenDoor>();
            changer.changeCostume();
        }
    }

    //public void removeSprout()
    //{
    //    currSprout.removeObject();
    //    movment.canMove = true;
    //    carryBox.addObject(currSprout.sproutType);
    //    carryBox.spawnPos.gameObject.SetActive(true);
    //    currCarryBoxObject++;
    //    playerStartGathering = false; 
    //}
    internal void setCapacityLevel(int capacityLevel)
    {
        this.capacityLevel = capacityLevel;
        foodHolder.carryLimit = capacityLevelAmount[--capacityLevel];
    }

    internal void setCollectSpeedLevel(int collectSpeedLevel)
    {
        this.collectSpeedLevel = collectSpeedLevel;
        refillElapsed = collectSpeedLevelAmount[--collectSpeedLevel];
        collectElapsed = collectSpeedLevelAmount[collectSpeedLevel];
    }

    internal void setSpeedLevel(int speedLevel)
    {
        this.speedLevel = speedLevel;
        movment.speed = speedLevelAmount[--speedLevel];
    }
    int CalculateGivenMoney(float totalPrice, float needed)

    {
        float timer = purchaseTimer;
        if (purchaseTimer < 0.02f)
            timer = 0.02f;
        float givePerTime = UnlockStationTimer / timer;
        int willGive = (int)(totalPrice / givePerTime);

        var money = GameSingleton.Instance.Money;

        if (money >= willGive && needed >= willGive)
        {
            purchaseElapsed = 0;
            return willGive;
        }
        else if (money >= 1000 && needed >= 1000)
        {
            purchaseElapsed = 0;
            return 1000;
        }
        else if (money >= 100 && needed >= 100)
        {
            purchaseElapsed = 0;
            return 100;
        }
        else if (money >= 10 && needed >= 10)
        {
            purchaseElapsed = 0;
            return 10;
        }
        else if (money >= 1 && needed >= 1)
        {
            purchaseElapsed = 0;
            return 1;
        }

        return 0;
    }
    //public int howManySpecificObjectHas(CarryObjectType carryObjectType)
    //{
    //    var firstCount = carryBox.carryBoxs.Where(x => x.first == carryObjectType && x.FirstLevelPrefeb.Any(x => x.activeInHierarchy == true)).Count();
    //    var secondCount = carryBox.carryBoxs.Where(x => x.second == carryObjectType && x.SecondLevelPrefeb.Any(x => x.activeInHierarchy == true)).Count();
    //    var thirtCount = carryBox.carryBoxs.Where(x => x.thirt == carryObjectType && x.ThirtLevelPrefeb.Any(x => x.activeInHierarchy == true)).Count();
    //    return firstCount + secondCount + thirtCount;
    //}

}

