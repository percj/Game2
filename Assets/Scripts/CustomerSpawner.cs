using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{

    public List<MarketOpener> markets;
    public List<MarketOpener> vipMarkets;
    float spawnTimer;
    float spawnTimer2;
    [Range(0,5)][SerializeField] float spawnElapsed;
    [Range(0,20)][SerializeField] float spawnElapsed2;
    public List<CustomerController> customers;
    public List<CustomerController> curCustomers;
    [SerializeField] GameObject vipCustomer;
    [SerializeField] List<OrderDTO> orders;
    [SerializeField] Transform startPos;
    [SerializeField] Transform tutorialStartPos;
    [SerializeField] Transform exitPos;
    [SerializeField] bool isSpawnerActive;
    [SerializeField] int areaMultiplier;


    // Update is called once per frame
    void Update()
    {
        SpawnCustomer();
        SpawnVipCustomer();
    }

    private void SpawnCustomer()
    {
        var selectedStation = markets.Where(x => x.gameObject.activeInHierarchy && x.hasCustomer == false && !x.dirtyDish.activeInHierarchy).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        if (isSpawnerActive && spawnTimer >= spawnElapsed && selectedStation != null)
        {
            spawnTimer = 0;
            var spawnedCustomer = customers[UnityEngine.Random.Range(0, customers.Count)];
            var x = Instantiate(spawnedCustomer.gameObject, transform);
            var customerController = x.GetComponent<CustomerController>();
            x.transform.parent = transform;
            x.transform.position = startPos.position;
            curCustomers.Add(customerController);
            customerController.exitPos = exitPos;
            customerController.markets = markets;
            customerController.orders = orders;
            customerController.areaMultiplier = areaMultiplier;
        }
        else
            spawnTimer += Time.deltaTime; 
    }

    private void SpawnVipCustomer()
    {
        var selectedStation = vipMarkets.Where(x => x.gameObject.activeInHierarchy && x.hasCustomer == false && !x.dirtyDish.activeInHierarchy).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        if (vipMarkets.Any(x => x.gameObject.activeInHierarchy) && isSpawnerActive && spawnTimer2 >= spawnElapsed2&& selectedStation != null)
        {
            spawnTimer2 = 0;
            var spawnedCustomer = vipCustomer;
            var x = Instantiate(spawnedCustomer, transform);
            var customerController = x.GetComponent<VIPCustomer>();
            x.transform.parent = transform;
            x.transform.position = startPos.position;
            customerController.exitPos = exitPos;
            customerController.markets = vipMarkets;
            customerController.orders = orders;
            customerController.areaMultiplier = areaMultiplier;
        }
        else
            spawnTimer2 += Time.deltaTime;
    }

    public void tutorialSpawn()
    {
        if (isSpawnerActive)
        {
            var spawnedCustomer = customers[UnityEngine.Random.Range(0, customers.Count)];
            var x = Instantiate(spawnedCustomer, transform);
            var customerController = x.GetComponent<CustomerController>();
            x.transform.parent = transform;
            x.transform.position = tutorialStartPos.position;
            customerController.exitPos = exitPos;
            customerController.markets = markets;
        }
    }
}
