using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperCardManager : MonoBehaviour
{
    [SerializeField] List<HireHelper> helperSpawner;
   // [SerializeField] HireHelper workerSpawner;
    [SerializeField] Card SpeedCard;
    [SerializeField] Card CollectSpeed;
    [SerializeField] Card Capacity;

    private void Start()
    {
        if (!SpeedCard.isLoaded)
            SpeedCard.LoadData();
        if (!CollectSpeed.isLoaded)
            CollectSpeed.LoadData();
        if (!Capacity.isLoaded)
            Capacity.LoadData();

        CapacityUpgraded();
        CollectSpeedUpgraded();
        SpeedUpgraded();
    }

    public void SpeedUpgraded()
    {
        helperSpawner.ForEach(x=>x.setHelperSpeedLevel(SpeedCard.currLevel));
        //workerSpawner.setHelperSpeedLevel(SpeedCard.currLevel);
    }
    public void CollectSpeedUpgraded()
    {
        helperSpawner.ForEach(x => x.setHelperCollectSpeedLevel(CollectSpeed.currLevel));
        //workerSpawner.setHelperCollectSpeedLevel(CollectSpeed.currLevel);
    }
    public void CapacityUpgraded()
    {
        helperSpawner.ForEach(x => x.setHelperCapacityLevel(Capacity.currLevel));
        //workerSpawner.setHelperCapacityLevel(Capacity.currLevel);
    }
}
