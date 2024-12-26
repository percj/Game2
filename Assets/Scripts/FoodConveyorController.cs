using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using UnityEngine.UI;
public class FoodConveyorController : MonoBehaviour
{
    [SerializeField] SplineFollower follower;
    public ConveyorController conveyorController;
    public OrderDTO orderDTO;
    NodeController currNode;
    [SerializeField] Image FillerImage;
    public bool inTimer;
    public float currTimer;
    float currEllapsed;
    
    // Update is called once per frame
    void Update()
    {
        if(follower != null)
            follower.onNode += OnEnterStation;
        if (!inTimer && currTimer >= 0)
        {
            SetTimer(-Time.deltaTime, currEllapsed);
        }
        
    }
    private void OnEnterStation(List<SplineTracer.NodeConnection> passed)
    {
        var connectedNode = passed[0].node.GetComponent<NodeController>();
        if(connectedNode != null)
        {
            if (connectedNode.nextNodeController != null && !connectedNode.nextNodeController.isFull)
                follower.followSpeed = 1;
            else if(currNode == null)
            {
                connectedNode.isFull = true;
                connectedNode.currFood = this;
                follower.followSpeed = 0;
                currNode = connectedNode;
            }
            else if(currNode == connectedNode)
            {
                connectedNode.isFull = true;
                connectedNode.currFood = this;
                follower.followSpeed = 0;
                currNode = connectedNode;
            }
        }
        
    }

    public void TryToGoNext()
    {
        if(currNode.nextNodeController != null && !currNode.nextNodeController.isFull)
        {
            follower.followSpeed = 1;
            currNode.isFull = false;
            currNode.currFood = null;
            currNode = currNode.nextNodeController;
            currNode.isFull = true;
            currNode.currFood = this;
        }
    }
    public void Remove()
    {
        if(currNode != null)
        {
            currNode.isFull = false;
            conveyorController.nodes.ForEach(x => { if (x.currFood != null) x.currFood.TryToGoNext(); });
        }
        conveyorController.currFoodCount--;
        Destroy(gameObject);
    }
    public void SetTimer(float time,float ellapsed)
    {
        currEllapsed = ellapsed;
        currTimer += time;
        FillerImage.fillAmount = currTimer / ellapsed;
    }
}
