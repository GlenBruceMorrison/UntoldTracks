using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tracks.Inventory;
using UnityEngine.Events;

namespace Tracks.Resource
{
    [CreateAssetMenu(fileName = "Item", menuName = "Data/ResourceHolder")]
    public class ResourceHolder : ScriptableObject
    {
        public List<ResourceContainer> CanProduce = new List<ResourceContainer>();

        public Item itemToHarvest;

        public int capacity = 1;

        public AudioClip harvestAudio;
        public AudioClip fullyHarvestedAudio;


        public UnityAction OnHarvested;

        // maybe should be an inventory for return type???
        public List<ItemContainer> Harvest()
        {
            if (capacity < 1)
            {
                return null;
            }

            var result = new List<ItemContainer>();

            foreach (var resource in CanProduce)
            {
                var roll = Random.Range(0f, 1f);

                if (roll >= (1 - resource.rarity))
                {
                    var container = new ItemContainer(null, -1);
                    container.FillAndReturnRemaining(resource.item, resource.baseAmount);

                    for (int i=0; i<resource.maxPossible; i++)
                    {
                        var multipleItemRoll = Random.Range(0f, 1f);

                        if (multipleItemRoll >= (1 - resource.multipleChance))
                        {
                            container.FillAndReturnRemaining(resource.item, Random.Range(1, resource.baseAmount));
                        }
                    }

                    result.Add(container);
                }
            }

            if (result.Count > 0)
            {
                capacity -= 1;

                if (capacity <= 0)
                {
                    OnHarvested?.Invoke();
                }
            }

            return result;
        }
    }

    [System.Serializable]
    public class ResourceContainer
    {
        public Item item;
        public float rarity = 1;
        public int baseAmount = 1;
        public bool canProductMultiple = false;
        public int maxPossible = 1;
        public float multipleChance = 1;
    }
}