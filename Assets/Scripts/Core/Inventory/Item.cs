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

    [Header("Tool Settings")]
    public bool isTool;
    public int toolStrength = 1;
    public bool isBuildingTool = false;
    public bool hasCustomInteractionFrame = false;
    public GameObject toolPrefab;

    [Header("Placeable Settings")]
    public bool isPlaceable = false;
    public PlaceableEntity placeablePrefab;

    [Header("MISC")]
    public bool isFuel = false;
    public int fuelStrength = 1;
}
