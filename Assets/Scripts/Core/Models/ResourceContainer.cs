using UnityEngine;
using UntoldTracks.Models;

namespace UntoldTracks.Models
{
    [System.Serializable]
    public class ResourceContainer
    {
        [Tooltip("Base Settings")]
        public ItemModel item;
        public float rarity = 1;
        public int baseAmount = 1;

        [Tooltip("Extra Rolls")]
        public bool canProductMultiple = false;
        public int maxPossible = 1;
        public float multipleChance = 1;
    }
}