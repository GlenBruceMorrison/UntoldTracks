using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Database/Items")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items = new List<Item>();
}
