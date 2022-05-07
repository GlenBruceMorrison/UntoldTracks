using System.Collections;
using System.Collections.Generic;
using Tracks.Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Recipe")]
public class Recipe : ScriptableObject
{
    public List<ItemContainer> ingredients = new List<ItemContainer>();
    public ItemContainer produces;
}
