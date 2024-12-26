using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/OrderDTO", order = 1)]
public class OrderDTO : ScriptableObject
{
    public int ID;
    public string orderName;
    public Sprite orderIcon;
    public int orderPrice;
    public GameObject prefab;
    public CarryFoodType foodType;
}