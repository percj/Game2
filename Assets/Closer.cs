using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Closer : MonoBehaviour
{
    float tempMin;
    float tempMax;
    bool isStart;
    private void Start()
    {
        tempMin = Camera.main.fieldOfView-20;
        tempMax = Camera.main.fieldOfView;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            isStart = false;
            StartCoroutine(focus(tempMin));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isStart = false;
            StartCoroutine(focus(tempMax));
        }
    }
    IEnumerator focus(float cameraFOG)
    {
            yield return new WaitForSeconds(0.2f);
        isStart = true;
        while(isStart)
        {
            if (Camera.main.fieldOfView > cameraFOG)
                Camera.main.fieldOfView -= 0.5f;
            else if (Camera.main.fieldOfView < cameraFOG)
                Camera.main.fieldOfView += 0.5f;
            else
            {
                yield break;
            }
            yield return null;
        }
    }
}
