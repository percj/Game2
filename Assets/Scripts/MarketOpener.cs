using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketOpener : MonoBehaviour
{
    public int MarketId;
    [HideInInspector] public bool hasCustomer;
    [HideInInspector] public MoneyAreaController moneyAreaController;
    [HideInInspector] public CustomerController currCustomer;
    [HideInInspector] public VIPCustomer currVIPCustomer;
    public Transform intractPos;
    public Transform sitPos;
    public Transform lookingPos;
    public bool currHelper;
    [SerializeField] GameObject FastFood;
    public ChiefController chiefController;
    [SerializeField] GameObject IceCream;
    [SerializeField] GameObject Pizza;
    [SerializeField] GameObject Steak;
    public GameObject dirtyDish;
    private void Awake()
    {
        clearTable();
    }
    public void clearTable()
    {
        dirtyDish.SetActive(false);
        FastFood.SetActive(false);
        IceCream.SetActive(false);
        Pizza.SetActive(false);
        Steak.SetActive(false);
    }
    internal void addFood()
    {
        switch (currCustomer.selectedFoodEnum)
        {
            case CarryFoodType.FastFood:
                FastFood.SetActive(true);
                break;
            case CarryFoodType.IceCream:
                IceCream.SetActive(true);
                break;
            case CarryFoodType.Pizza:
                Pizza.SetActive(true);
                break;
            case CarryFoodType.Steak:
                Steak.SetActive(true);
                break;
        }
    }
    internal void addVIPFood(CarryFoodType x)
    {
        switch (x)
        {
            case CarryFoodType.FastFood:
                FastFood.SetActive(true);
                break;
            case CarryFoodType.IceCream:
                IceCream.SetActive(true);
                break;
            case CarryFoodType.Pizza:
                Pizza.SetActive(true);
                break;
            case CarryFoodType.Steak:
                Steak.SetActive(true);
                break;
        }
    }
    public void exitCustomerToTable()
    {
        clearTable();
        dirtyDish.SetActive(true);
    }
}
