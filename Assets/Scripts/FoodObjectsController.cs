using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CarryFoodType
{
    DirtyDish,
    FastFood,
    IceCream,
    Pizza,
    Steak
}
public class FoodObjectsController : MonoBehaviour
{
    [HideInInspector] public int boxLevel;
    public List<GameObject> FirstLevelPrefeb;
    public List<GameObject> SecondLevelPrefeb;
    public List<GameObject> ThirtLevelPrefeb;

    [HideInInspector] public CarryFoodType? first;
    [HideInInspector] public CarryFoodType? second;
    [HideInInspector] public CarryFoodType? third;

    public void clear()
    {
        first = null;
        second = null;
        third = null;
        foreach (var go in FirstLevelPrefeb)
        {
            go.SetActive(false);
        }
        foreach (var go in SecondLevelPrefeb)
        {
            go.SetActive(false);
        }
        foreach (var go in ThirtLevelPrefeb)
        {
            go.SetActive(false);
        }
    }

    public void addFirst(CarryFoodType food)
    {
        first = food;
        foreach (var go in FirstLevelPrefeb)
        {
            if (go.name == food.ToString())
            {
                go.SetActive(true);
                break;
            }
        }

    }
    public void addSecond(CarryFoodType food)
    {
        first = food;
        foreach (var go in SecondLevelPrefeb)
        {
            if (go.name == food.ToString())
            {
                go.SetActive(true);
                break;
            }
        }

    }
    public void addThird(CarryFoodType food)
    {
        first = food;
        foreach (var go in ThirtLevelPrefeb)
        {
            if (go.name == food.ToString())
            {
                go.SetActive(true);
                break;
            }
        }

    }
}
