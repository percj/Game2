using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;



public class HeadHelperController : MonoBehaviour
{
    public List<MarketOpener> orderPos;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    public Transform WaitPos;
    MarketOpener selectedStation;
    bool going;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<NavMeshAgent>().enabled = true;
        agent.SetDestination(WaitPos.position);
    }

    // Update is called once per frame
    void Update()
    {
        FindTable();
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
        }

        //  animator.SetBool("Carry", carryObjects.carryObjets.Count>0);

    }
    void FindTable()
    {
        if (going && selectedStation != null && selectedStation.currCustomer != null && selectedStation.currCustomer.handUp) return;
        selectedStation = orderPos.Where(x => x.currCustomer != null && x.currCustomer.handUp).FirstOrDefault();
        if (selectedStation == null)
            agent.SetDestination(WaitPos.position);
        else
        {
            going = true;
            agent.SetDestination(selectedStation.intractPos.position);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "forOrder")
        {
            var markett = other.GetComponent<MarketOpener>();
            if (markett.currCustomer != null)
            {
                if (markett.currCustomer != null)
                {
                    if (markett.currCustomer.handUp)
                    {
                        markett.currCustomer.handUp = false;
                        going = false;
                    }
                }

            }
        }
    }
}

