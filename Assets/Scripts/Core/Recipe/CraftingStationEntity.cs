using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UntoldTracks
{
    public class CraftingStationEntity : Entity
    {
        public CraftingStationData _data;
    }

    [CreateAssetMenu(fileName = "CraftingStation", menuName = "Data/CraftingStation")]
    public class CraftingStationData : ScriptableObject
    {
        
        public Item item;
        public CraftingStationEntity worldObject;
        public List<Recipe> recipes = new List<Recipe>();
    }
}
