using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class ConveyorController : MonoBehaviour
{
    public int currFoodCount;
    public int maxFoodCount;
    public List<NodeController> nodes;
    public bool ControllConveyor()
    {
        return maxFoodCount <= currFoodCount;
    }
}
