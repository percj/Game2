using System.Collections.Generic;
using UnityEngine;

public class kitchenDoor : MonoBehaviour
{
    public GameObject OpenedObject;
    public List<GameObject> ClosedObjects;

    private void Awake()
    {
        var name = PlayerPrefs.GetString("Costume", "");
        if (OpenedObject.name == name)
            changeCostume();
    }

    public void changeCostume()
    {
        OpenedObject.SetActive(true);
        ClosedObjects.ForEach(x => x.SetActive(false));
        PlayerPrefs.SetString("Costume", OpenedObject.name);
    }
}
