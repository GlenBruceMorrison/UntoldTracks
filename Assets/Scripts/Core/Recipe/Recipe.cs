using System.Collections;
using System.Collections.Generic;
using UntoldTracks.Inventory;
using UnityEngine;

namespace UntoldTracks
{
    [CreateAssetMenu(fileName = "Item", menuName = "Data/Recipe")]
    public class Recipe : ScriptableObject
    {
        public List<ItemContainer> ingredients = new List<ItemContainer>();

        public ItemContainer produces;

        public bool CanCreate(IInventory inventory)
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
