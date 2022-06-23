using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UntoldTracks.Models
{
    [CreateAssetMenu(fileName = "Item", menuName = "Data/Item")]
    public class ItemModel : SerializableScriptableObject
    {
        [Header("Display Data")]
        public string displayName;
        public Sprite sprite;
        [TextArea()] public string description;

        [Space(20)]
        [Header("Stack Settings")]
        public bool stackable = true;
        public int stackSize = 20;

        [Space(20)]
        [Header("Equipable Settings")]
        public bool isEquipable;
        public EquipableEntityBase equipablePrefab;
        public bool hasCustomInteractionFrame = false;
        public bool degradable = false;
        public int durability = 0;

        [Space(20)]
        [Header("Placeable Settings")]
        public bool isPlaceable = false;
        public PlaceableEntity placeablePrefab;
        public bool canRotate = true;
    }
}