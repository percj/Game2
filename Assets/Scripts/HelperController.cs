using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HelperController : MonoBehaviour
{
    [SerializeField] List<float> speedLevelAmount;
    [SerializeField] List<float> collectSpeedLevelAmount;
    [SerializeField] List<int> capacityLevelAmount;
    [SerializeField] NavMeshAgent agent;
    public Transform Collect;
    public Transform WaitPos;
    [SerializeField] Animator animator;
    public List<MarketOpener> tablePos;
    float collectTimer;
    [Range(0, 5f)][SerializeField] float collectElapsed;
    float refillTimer;
    [Range(0, 5f)][SerializeField] float refillElapsed;
    MarketOpener selectedStation;
    [SerializeField] FoodHolder foodHolder;
    public ConveyorController conveyor;
    bool canIntract;
    bool hasDirty;
    [SerializeField] GameObject dirtyPlate;
    public Transform sink;
    private bool onWay;


    public int collectSpeedLevel;
    public int speedLevel;
    public int capacityLevel;

    bool collectFull;

    [SerializeField] List<OrderDTO> currFood = new List<OrderDTO>();


    void Start()
    {
        GetComponent<NavMeshAgent>().enabled = true;
        setSpeedLevel(speedLevel);
    }

    void Update()
    {
        if (selectedStation == null) findStation();
        else
        {
            if (selectedStation.currCustomer == null)
            {
                if (!selectedStation.dirtyDish.activeInHierarchy)
                {
                    if (selectedStation.currHelper)
                        selectedStation.currHelper = false;

                    selectedStation = null;
                }
            }
            else if (!selectedStation.currCustomer.wantFood)
            {
                if (selectedStation.currHelper)
                    selectedStation.currHelper = false;

                selectedStation = null;
            }
        }
        AnimationControl();
    }
    private void AnimationControl()
    {
        if (Vector3.Distance(agent.destination, transform.position) > 0.2f)
        {
            animator.SetBool("run", true);
            agent.isStopped = false;
        }
        else
        {
            animator.SetBool("run", false);
            agent.isStopped = true;
            onWay = false;
        }
    }

    void findStation()
    {
        if(foodHolder.carryObjects.Count != 0 && collectFull && !hasDirty && !onWay)
        {
            var selectedStations = tablePos.Where(x=> x.currCustomer != null && x.currCustomer.wantFood && 
            foodHolder.carryObjects.Contains(x.currCustomer.selectedFoodEnum) && !x.currHelper
            ).OrderBy(x => Guid.NewGuid()).Take(1).ToList();
            if (selectedStations.Count == 0)
            {
                agent.SetDestination(WaitPos.position);
                onWay = true;
            }
            else
            {
                selectedStation = selectedStations[0];
                selectedStation.currHelper = true;
                agent.SetDestination(selectedStation.intractPos.position);
                onWay = true;
            } 
        }
        else if(hasDirty && collectFull && !onWay)
        {
            agent.SetDestination(sink.position);
            onWay = true;
        }
        else
        {
            if (selectedStation != null && !selectedStation.dirtyDish.activeInHierarchy) { selectedStation = null; onWay = false; }
            var selectedStationss = tablePos.Where(x => x.currCustomer == null && x.dirtyDish.activeInHierarchy && !x.currHelper).OrderBy(x => Guid.NewGuid()).Take(1).ToList();
            if (selectedStationss.Count == 0 && !hasDirty && !onWay)
            {
                collectFull = false;
                agent.SetDestination(Collect.position);
                onWay = true;
            }
            else if (selectedStationss.Count == 0 && hasDirty && !onWay)
            {
                agent.SetDestination(sink.position);
                onWay = true;
            }
            else if (!onWay && (foodHolder.carryObjects.Count == 0 || foodHolder.countTray(CarryFoodType.DirtyDish) > 0))
            {
                selectedStation = selectedStationss[0];
                selectedStation.currHelper = true;
                agent.SetDestination(selectedStation.intractPos.position);
                onWay = true;
            }
        }   
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Refill" && foodHolder.carryObjects.Contains(CarryFoodType.DirtyDish))
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
                    carryStation.goToTrash(x,false);
                }
            }
            if (foodHolder.countTray(CarryFoodType.DirtyDish) == 0)
            {
                findStation();
                onWay = false;
                hasDirty = false;
                collectFull = false;
            }
        }
        if (other.gameObject.tag == "forOrder")
        {
            var markett = other.GetComponent<MarketOpener>();
            if (markett.currCustomer != null && Vector3.Distance(markett.intractPos.position, agent.destination) < 0.5f)
            {
                if (markett.currCustomer.wantFood && foodHolder.carryObjects.Contains(markett.currCustomer.wantFoodType))
                {
                    markett.currCustomer.wantFood = false;
                    markett.addFood();
                    foodHolder.removeObject(markett.currCustomer.wantFoodType);
                    onWay = false;
                }
            }
            else
            {
                if (markett.dirtyDish.activeInHierarchy && foodHolder.carryObjects.Count < foodHolder.carryLimit && Vector3.Distance(markett.intractPos.position, agent.destination) < 0.5f)
                {
                    markett.clearTable();
                    foodHolder.addObject(CarryFoodType.DirtyDish);
                    collectFull = false;
                    if (foodHolder.carryObjects.Count == foodHolder.carryLimit)
                        collectFull = true;
                    hasDirty = true;
                    onWay = false;
                }
            }
        }
        if (other.gameObject.tag == "Food")
        {
            var foodConveyorController = other.GetComponent<FoodConveyorController>();
            if (foodHolder.carryObjects.Count < foodHolder.carryLimit)
            {
                refillTimer += Time.deltaTime;
                if (refillTimer > refillElapsed)
                {
                    refillTimer = 0;
                    foodHolder.addObject(foodConveyorController.orderDTO.foodType);
                    foodConveyorController.Remove();
                }
                else
                {
                    foodConveyorController.SetTimer(Time.deltaTime, refillElapsed);
                    foodConveyorController.inTimer = true;
                }
            }
            if (foodHolder.carryObjects.Count == foodHolder.carryLimit || (conveyor.currFoodCount == 0 && foodHolder.carryObjects.Count !=0))
            {
                findStation();
                onWay = false;
                collectFull = true;
            }
        }
    }

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
        agent.speed = speedLevelAmount[--speedLevel];
    }
}
