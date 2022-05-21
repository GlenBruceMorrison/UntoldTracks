using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item")]
public class Item : ScriptableObject
{
    [Header("Display Data")]
    public string name;
    public Sprite sprite;
    [TextArea()]
    public string description;

    [Header("Stack Settings")]
    public bool stackable = true;
    public int stackSize = 20;
    public bool degradable = false;
    public int durability = 0;

    [Header("Tool Settings")]
    public bool isTool;
    public int toolStrength = 1;
    public bool isBuildingTool = false;
    public bool hasCustomInteractionFrame = false;
    public GameObject toolPrefab;

    [Header("Placeable Settings")]
    public bool isPlaceable = false;
    public PlaceableEntity placeablePrefab;
    public bool canRotate=true;

    [Header("MISC")]
    public bool isFuel = false;
    public int fuelStrength = 1;
}

public class ItemInstance
{
    private Item _data;
    private float _currentDurability;

    public ItemInstance(Item data)
    {
        _data = data;
    }

    public Item Item
    {
        get
        {
            return _data;
        }
        set
        {
            _data = value;
        }
    }

    public float CurrentDurability
    {
        get
        {
            return _currentDurability;
        }
    }

    public int MaxDurability
    {
        get
        {
            return _data.durability;
        }
    }

    public bool DoesDegrage
    {
        get
        {
            return _data.degradable;
        }
    }

    public bool DoesStack
    {
        get
        {
            return _data.stackable;
        }
    }

    public int StackSize
    {
        get
        {
            return _data.stackSize;
        }
    }
}
