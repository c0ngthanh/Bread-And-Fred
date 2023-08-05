using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;
[CreateAssetMenu(menuName = "Item")]
public class ItemShopData : ScriptableObject
{
    [Header("Item Info")]
    [SerializeField] public string itemName;
    [SerializeField] public SellBy sellBy;
    [SerializeField] public TypeSell typeSell;
    [SerializeField] public Sprite iconItemSprite;
    [SerializeField] public Sprite[] levelBarListSprite;
    [SerializeField] public float[] price;
    [SerializeField] public string itemDescription;
    [Header("Value per Unit (%)")]
    [SerializeField] public float percentIncrease;
    [SerializeField] public List<List<int>> listOfLists = new List<List<int>>();
}
