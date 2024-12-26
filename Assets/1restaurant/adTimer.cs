using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class adTimer : MonoBehaviour
{
    [SerializeField] GameObject ad;
    [SerializeField] GameObject timer;
    [SerializeField] Image timerFiller;
    [SerializeField] float countdownValue;
    [SerializeField] float nextAddWaitSecond;

    void OnEnable()
    {
        currCountdownValue = countdownValue;
        StartCoroutine(StartCountdown());
    }

    float currCountdownValue;
    public IEnumerator StartCountdown()
    {
        var CountdownValue = currCountdownValue;
        while (CountdownValue > 0)
        {
            CountdownValue -= Time.deltaTime;
            timerFiller.fillAmount = CountdownValue/currCountdownValue;
            yield return null;
        }
        GameSingleton.Instance.multiplier = 1;
        timer.SetActive(false);
        yield return new WaitForSeconds(nextAddWaitSecond);
        ad.SetActive(true);
        gameObject.SetActive(false);
        timer.SetActive(true);
    }
}
