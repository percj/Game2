using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class ChiefController : MonoBehaviour
{
    [SerializeField] ConveyorController conveyorController;
    
    [SerializeField] SplineComputer computer;
    public List<OrderDTO> waitingOrderDTOs ;
    [SerializeField] Animator anim;
    Vector3 startPOS;
    bool isStarted;

    void Start()
    {
        startPOS = transform.position;
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
   void Update()
    {
      
        if (!isStarted && waitingOrderDTOs.Count>0 && !conveyorController.ControllConveyor())
        {
            StartCoroutine(StartCooking());

        }
        else if (!isStarted)
        {
            transform.position = startPOS;
            transform.rotation = new Quaternion(0,0,0,0);
        }

    }
    IEnumerator StartCooking()
    {
        isStarted = true;
        transform.position = startPOS;
        anim.SetBool("Cooking", true);
        while(anim.GetBool("Cooking")) yield return null;
        transform.position = startPOS;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        var x = Instantiate(waitingOrderDTOs[0].prefab, null);
        x.GetComponent<SplineFollower>().spline = computer;
        x.GetComponent<FoodConveyorController>().orderDTO = waitingOrderDTOs[0];
        x.GetComponent<FoodConveyorController>().conveyorController = conveyorController;
        waitingOrderDTOs.RemoveAt(0);
        conveyorController.currFoodCount++; 
        isStarted = false;
        yield return null;
    }

    public void FinishCooking()
    {
        anim.SetBool("Cooking", false);
    }
    public void TakeOrder(OrderDTO orderDTO)
    {
        waitingOrderDTOs.Add(orderDTO);
    }
}

