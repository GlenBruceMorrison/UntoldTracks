using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UnityEngine;

namespace UntoldTracks.Models
{
    [CreateAssetMenu(fileName = "Item", menuName = "Data/Recipe")]
    public class Recipe : SerializableScriptableObject
    {
        public List<ItemContainer> ingredients = new List<ItemContainer>();

        public ItemContainer produces;

        public bool CanCreate(Inventory inventory)
        {
            foreach (var ingredient in ingredients)
            {
                if (!inventory.CanTake(ingredient.Item, ingredient.Count))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
