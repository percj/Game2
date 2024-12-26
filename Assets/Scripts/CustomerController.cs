using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;



public class CustomerController : MonoBehaviour
{
    [HideInInspector] public MarketOpener selectedStation;
    [HideInInspector] public List<MarketOpener> markets;
    public List<OrderDTO> orders;
    [HideInInspector] public Transform exitPos;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [HideInInspector] public CarryFoodType selectedFoodEnum;

    [SerializeField] GameObject smoke;
    [SerializeField] GameObject moneyCoin;

    bool willExit;
    bool wanderState = false;
    bool inStation = false;

    [Header("=== Frustrade Logic ===")]
    [SerializeField] GameObject frustrateCanvas;
    [SerializeField] Image frustrate;
    [SerializeField] Image frustrateActionImage;
    [SerializeField] Sprite Thinking;
    [SerializeField] Sprite WantToGiveOrder;
    [SerializeField] GameObject frustrateAngry;
    [Range(0, 20)][SerializeField] float frustrateElapsed;
    float frustrateTimer;
    [HideInInspector] public OrderDTO selectedOrder;
    [HideInInspector] public bool handUp;
    [HideInInspector] public bool wantFood;
    [HideInInspector] public CarryFoodType wantFoodType;
    
    [HideInInspector] public int areaMultiplier;

    //[Header("=== Selection ===")]
    //[HideInInspector] public CarryObjectType selectedOrderObjects;
    //[HideInInspector] public List<CarryObjectType> openedLand = new List<CarryObjectType>();


    void Start()
    {
        selectedOrder = orders.OrderBy(x => Guid.NewGuid()).First();
        selectedFoodEnum = (CarryFoodType)Enum.Parse(typeof(CarryFoodType), selectedOrder.orderName);
        selectedStation = markets.Where(x => x.gameObject.activeInHierarchy && x.hasCustomer == false && !x.dirtyDish.activeInHierarchy).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        if (selectedStation != null)
        {
            selectedStation.hasCustomer = true;
            agent.SetDestination(selectedStation.intractPos.position);
        }
        else
        {
            selectedStation = null;
            agent.SetDestination(exitPos.position);
            willExit = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
          AnimationControl();

        if (!wanderState && inStation && !willExit)
        {
            StartCoroutine(WanderState());
        }
    }
    IEnumerator WanderState()
    {
        agent.enabled = false;
        transform.position = selectedStation.sitPos.position;
        agent.enabled = true;
        transform.LookAt(selectedStation.lookingPos);
        wanderState = true;
        frustrateCanvas.SetActive(true);
        frustrateActionImage.sprite = Thinking;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1, 6));
        animator.SetTrigger("waitingOrder");
        handUp = true;
        frustrateActionImage.color = new Color(0.7f, 0.7f, 0.7f, 1);
        frustrateActionImage.sprite = WantToGiveOrder;
        while(frustrateTimer < frustrateElapsed && handUp)
        {
            frustrateTimer += Time.deltaTime;
            frustrate.fillAmount = frustrateTimer / frustrateElapsed;
            yield return null;
        }
        if (!handUp)
        {
            frustrateTimer = 0;
            animator.SetTrigger("giveOrder");
            selectedStation.chiefController.TakeOrder(selectedOrder);
            frustrateActionImage.sprite = selectedOrder.orderIcon;
            wantFood = true;
            wantFoodType = selectedOrder.foodType;
            frustrateCanvas.SetActive(true);
            while (wantFood)
            {
                frustrate.fillAmount = frustrateTimer / frustrateElapsed;
                yield return null;
            }
            //while (frustrateTimer < frustrateElapsed && wantFood)
            //{
            //    frustrateTimer += Time.deltaTime;
            //    frustrate.fillAmount = frustrateTimer / frustrateElapsed;
            //    yield return null;
            //}

            if (!wantFood)
            {
                animator.SetTrigger("startEating");
                var x = Instantiate(smoke, selectedStation.lookingPos);
                x.transform.position += Vector3.up;
                frustrateCanvas.SetActive(false);
                yield return new WaitForSeconds(9f);
                animator.SetTrigger("endEating");
                Destroy(x);
                selectedStation.moneyAreaController.addMoney(selectedOrder.orderPrice * areaMultiplier);
                selectedStation.exitCustomerToTable();
                exiting();
                yield break;
            }
        }

        angryExiting();
        yield return null;
    }

    private void angryExiting()
    {
        animator.SetTrigger("giveOrder");
        frustrateAngry.SetActive(true);
        exiting();
    }
    private void exiting()
    {
        selectedStation.hasCustomer = false;
        selectedStation.currCustomer = null;
        agent.enabled = false;
        transform.position = selectedStation.intractPos.position;
        agent.enabled = true;
        agent.SetDestination(exitPos.position);
        willExit = true;
    }

    private void AnimationControl()
    {
        if (Vector3.Distance(agent.destination, transform.position) > 0.1f)
        {
            animator.SetBool("run", true);
            animator.SetBool("sitting", false);
            agent.isStopped = false;
        }
        else
        {
            if (selectedStation != null && !wanderState)
            {
                animator.SetBool("sitting", true);
                inStation = true;
                selectedStation.currCustomer = GetComponent<CustomerController>();
            }
            animator.SetBool("run", false);
            agent.isStopped = true;
            if (willExit)
            {
                Destroy(gameObject);
                return;
            }

        }
    }
}
