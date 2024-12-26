using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class VIPCustomer : MonoBehaviour
{
    [HideInInspector] public MarketOpener selectedStation;
    [HideInInspector] public List<MarketOpener> markets;
    public List<OrderDTO> orders;
    [HideInInspector] public Transform exitPos;
    [SerializeField] UnityEngine.AI.NavMeshAgent agent;
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
    [SerializeField] Image order1;
    [SerializeField] Image order2;
    [SerializeField] Image order3;
    [SerializeField] GameObject frustrateAngry;
    [Range(0, 20)][SerializeField] float frustrateElapsed;
    float frustrateTimer;
    [HideInInspector] public List<OrderDTO> selectedOrders;
    [HideInInspector] public bool handUp;
    [HideInInspector] public bool wantFood;
    [HideInInspector] public List<CarryFoodType> wantFoodType;

    [HideInInspector] public int areaMultiplier;

    //[Header("=== Selection ===")]
    //[HideInInspector] public CarryObjectType selectedOrderObjects;
    //[HideInInspector] public List<CarryObjectType> openedLand = new List<CarryObjectType>();


    void Start()
    {
        wantFoodType = new List<CarryFoodType>();
        selectedOrders = orders.OrderBy(x => Guid.NewGuid()).Take(3).ToList();
        selectedStation = markets.Where(x => x.gameObject.activeInHierarchy && x.hasCustomer == false && !x.dirtyDish.activeInHierarchy).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        if (selectedStation != null)
        {
            selectedStation.hasCustomer = true;
            selectedStation.currVIPCustomer =this;
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
        frustrateActionImage.sprite = WantToGiveOrder;
        while (frustrateTimer < frustrateElapsed && handUp)
        {
            frustrateTimer += Time.deltaTime;
            frustrate.fillAmount = frustrateTimer / frustrateElapsed;
            yield return null;
        }
        if (!handUp)
        {
            frustrateTimer = 0;
            animator.SetTrigger("giveOrder");
            for(int i = 0;i<selectedOrders.Count;i++)
            {
                wantFoodType.Add(selectedOrders[i].foodType);
                selectedStation.chiefController.TakeOrder(selectedOrders[i]);
            }
            order1.color = new Color(0.7f, 0.7f, 0.7f, 1);
            order2.color = new Color(0.7f, 0.7f, 0.7f, 1);
            order3.color = new Color(0.7f, 0.7f, 0.7f, 1);


            order1.sprite = selectedOrders[0].orderIcon;
            order2.sprite = selectedOrders[1].orderIcon;
            order3.sprite = selectedOrders[2].orderIcon;
            order1.gameObject.SetActive(true);
            order2.gameObject.SetActive(true);
            order3.gameObject.SetActive(true);

            frustrateActionImage.gameObject.SetActive(false);
            wantFood = true;
            frustrateCanvas.SetActive(true);
            float waiting = 0;
            while (waiting < 7 && wantFood)
            {
                waiting += Time.deltaTime;
                frustrate.fillAmount = frustrateTimer / frustrateElapsed;
                yield return null;
            }
            while (frustrateTimer < frustrateElapsed && wantFood)
            {
                frustrateTimer += Time.deltaTime;
                frustrate.fillAmount = frustrateTimer / frustrateElapsed;
                yield return null;
            }
            if (!wantFood)
            {
                animator.SetTrigger("startEating");
                var x = Instantiate(smoke, selectedStation.lookingPos);
                frustrateCanvas.SetActive(false);
                yield return new WaitForSeconds(9f);
                animator.SetTrigger("endEating");
                Destroy(x);
                Invoke("forceMoney", 0f);
                Invoke("forceMoney", 0.1f);
                Invoke("forceMoney", 0.2f);
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
        order1.gameObject.SetActive(false);
        order2.gameObject.SetActive(false);
        order3.gameObject.SetActive(false);
        selectedStation.clearTable();
        animator.SetTrigger("giveOrder");
        frustrateAngry.SetActive(true);
        exiting();
    }
    private void exiting()
    {
        selectedStation.hasCustomer = false;
        selectedStation.currVIPCustomer = null;
        agent.enabled = false;
        transform.position = selectedStation.intractPos.position;
        agent.enabled = true;
        agent.SetDestination(exitPos.position);
        willExit = true;
    }
    private void forceMoney()
    {
        var selected = selectedOrders.First();
        var x = Instantiate(moneyCoin, selectedStation.sitPos);
        x.transform.position = selectedStation.sitPos.position;
        x.gameObject.GetComponent<moneyCoin>().priceValue = selected.orderPrice * 3 * areaMultiplier;
        var right = Vector3.right * UnityEngine.Random.Range(-1, 1);
        var forward = Vector3.forward * 1.5f;
        x.GetComponent<Rigidbody>().AddForce(((Vector3.up * 3.5f) + forward + right) * 2f, ForceMode.Impulse);
        selectedOrders.Remove(selected);
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
