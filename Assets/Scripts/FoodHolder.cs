using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodHolder : MonoBehaviour
{
    [Range(1, 6)] public int carryLimit;
    [SerializeField] public List<CarryFoodType> carryObjects;
    [SerializeField] public List<Transform> carryObjectsReferansPos;
    [SerializeField] FoodObjectsController firstTray;
    [SerializeField] FoodObjectsController secondTray;
    [SerializeField] Animator anim;

    private void Start()
    {
        UpdateTrayPos();
    }
    public void addObject(CarryFoodType x)
    {
        if (carryObjects.Count >= carryLimit) return;
        carryObjects.Add(x);
        UpdateTrayPos();
    }
    public void UpdateTrayPos()
    {
        firstTray.clear();
        secondTray.clear();
        carryObjectsReferansPos.Clear();
        anim.SetInteger("CarryCount", carryObjects.Count);
        if (carryObjects.Count > 0)
        {
            firstTray.gameObject.SetActive(true);
            anim.SetBool("Carry", true);
        }
        else
        {
            firstTray.gameObject.SetActive(false);
            anim.SetBool("Carry", false);
        }
        if (carryObjects.Count > 3)
        {
            secondTray.gameObject.SetActive(true);
            //anim.SetBool("TwoArm", true);
        }
        else
        {
            secondTray.gameObject.SetActive(false);
            // anim.SetBool("TwoArm", false);
        }
        for(int x = 0; x <carryObjects.Count; x++)
        {
            if (carryObjects.Count > x)
            {
                if (x == 0)
                {
                    firstTray.addFirst(carryObjects[x]);
                    carryObjectsReferansPos.Add(firstTray.FirstLevelPrefeb[0].transform);
                }
                else if (x == 1)
                {
                    firstTray.addSecond(carryObjects[x]);
                    carryObjectsReferansPos.Add(firstTray.SecondLevelPrefeb[0].transform);
                }
                else if (x == 2)
                {
                    firstTray.addThird(carryObjects[x]);
                    carryObjectsReferansPos.Add(firstTray.ThirtLevelPrefeb[0].transform);
                }
                else if (x == 3)
                {
                    secondTray.addFirst(carryObjects[x]);
                    carryObjectsReferansPos.Add(secondTray.FirstLevelPrefeb[0].transform);
                }
                else if (x == 4)
                {
                    secondTray.addSecond(carryObjects[x]);
                    carryObjectsReferansPos.Add(secondTray.SecondLevelPrefeb[0].transform);
                }
                else if (x == 5)
                {
                    secondTray.addThird(carryObjects[x]);
                    carryObjectsReferansPos.Add(secondTray.ThirtLevelPrefeb[0].transform);
                }
            }
        }
    }


    public Vector3 removeObject(CarryFoodType x)
    {
        if (carryObjects.Count <= 0) return Vector3.zero;
        var index = carryObjects.IndexOf(x);
        carryObjects.Remove(x);
        var returnPos = carryObjectsReferansPos.ElementAt(index);
        carryObjectsReferansPos.RemoveAt(index);
        UpdateTrayPos();
        return returnPos.position;
    }

    public bool controllTray(CarryFoodType x)
    {
        return carryObjects.Contains(x);
    }
    public int countTray(CarryFoodType type)
    {
        return carryObjects.Where(x => x == type).Count();
    }
}
