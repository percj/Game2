using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForEnable : MonoBehaviour
{
    [SerializeField] float enableTimer;
    [SerializeField] GameObject enableObject;

    void Start()
    {
        StartCoroutine(timer());
    }
    IEnumerator timer()
    {
        while (enableTimer >= 0)
        {
            enableTimer -= Time.deltaTime;
            enableObject.SetActive(false);
            yield return null;
        }
        enableObject.SetActive(true);
        Destroy(this);
        yield return null;
    }
    
}
