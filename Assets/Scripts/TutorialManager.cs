using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] CustomerSpawner spawner;
    [SerializeField] FoodHolder foodHolder;
    public GameObject[] popUps;
    private int popUpIndex;

    void Awake()
    {
        popUpIndex = PlayerPrefs.GetInt("TutorialIndex", popUpIndex);
    }
    void Start()
    {
        if(popUpIndex == 5)
            Destroy(gameObject);

        StartCoroutine(playTutorial());
    }

    private IEnumerator playTutorial()
    {
        popUpActivate();
        if (popUpIndex == 0)
        {
            while(true)
            {
                if (spawner.curCustomers.Any(x => x.wantFood))
                   break;
                yield return null;
            }
            popUpIndex++;
            popUpActivate();
        }
        if (popUpIndex == 1)
        {
            while (true)
            {
                if (foodHolder.carryObjects.Count > 0)
                    break;
                yield return null;
            }
            popUpIndex++;
            popUpActivate();
        }
        if (popUpIndex == 2)
        {
            while (true)
            {
                if (foodHolder.carryObjects.Count == 0)
                     break;
                yield return null;
            }
            popUpIndex++;
            popUpActivate();
        }
        if (popUpIndex == 3)
        {
            while (true)
            {
                if (foodHolder.carryObjects.Contains(CarryFoodType.DirtyDish))
                    break;
                yield return null;
            }
            popUpIndex++;
            popUpActivate();
        }
        if (popUpIndex == 4)
        {
            while (true)
            {
                if (!foodHolder.carryObjects.Contains(CarryFoodType.DirtyDish))
                    break;
                yield return null;
            }
            popUpIndex++;
            popUpActivate();
        }
        if (popUpIndex == 5)
        {
            Destroy(gameObject);
            yield break;
        }
    }

    private void popUpActivate()
    {
        PlayerPrefs.SetInt("TutorialIndex", popUpIndex);
        for (int i = 0; i < popUps.Length; i++)
        {
            popUps[i].SetActive(i == popUpIndex);
        }
    }
}
