using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item")]
public class Item : ScriptableObject
{
    public string name;
    public string description;
    public Sprite sprite;
    public bool stackable = true;
    public int stackSize = 20;
    public bool isBuildingTool = false;
    public bool isFuel = false;
    public int fuelStrength = 1;
}
