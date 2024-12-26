using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adErrorMsg : MonoBehaviour
{
    public GameObject adMsg;

    void OnEnable()
    {
        StartCoroutine(StartCountdown());
    }

    float currCountdownValue = 3;

    public IEnumerator StartCountdown()
    {
        currCountdownValue = 3;
        while (currCountdownValue > 0)
        {
            currCountdownValue -= Time.deltaTime;
            yield return null;
        }
        adMsg.SetActive(false);
        yield return null;
    }
}
