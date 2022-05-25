using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UnityEngine;

namespace UntoldTracks
{
    [CreateAssetMenu(fileName = "Item", menuName = "Data/RecipeBook")]
    public class RecipeBook : ScriptableObject
    {
        public string recipeBookTitle;
        public List<Recipe> recipes = new List<Recipe>();
    }
}
