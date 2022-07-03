using System.ComponentModel;
using UnityEngine;
using UntoldTracks.Models;

namespace UntoldTracks.Models
{
    [System.Serializable]
    public class ResourceContainer
    {
        [Header("Base Settings")]
        public ItemModel item;
        
        [Range(0, 1)]
        public float rarity = 1;
        
        [Range(0, 20)]
        public int baseAmount = 1;

        [Header("Extra Rolls")]
        [Range(0, 1)]
        public float multipleChance = 1;
        
        [Range(0, 20)]
        public int maxPossible = 1;
    }
}