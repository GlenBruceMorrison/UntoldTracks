using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Models;
using UntoldTracks;

namespace UntoldTracks.Models
{
    [CreateAssetMenu(fileName = "CraftingStation", menuName = "Data/CraftingStation")]
    public class StationModel : SerializableScriptableObject
    {
        [SerializeField] public ItemModel _item;
        [SerializeField] public List<Recipe> _recipes = new();

        public List<Recipe> Recipes
        {
            get
            {
                return _recipes;
            }
        }
    }
}
