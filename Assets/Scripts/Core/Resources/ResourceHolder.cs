using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UntoldTracks.Inventory;
using UnityEngine.Events;

namespace UntoldTracks.Resource
{
    [CreateAssetMenu(fileName = "Item", menuName = "Data/ResourceHolder")]
    public class ResourceHolder : ScriptableObject
    {
        public int durability = 5;
        public List<Item> toolsToHarvest = new List<Item>();

        public List<ResourceContainer> CanProduce = new List<ResourceContainer>();

        public AudioClip harvestAudio;
        public AudioClip fullyHarvestedAudio;

        public UnityAction OnHarvested;

        public bool IsItemValid(Item item)
        {
            return toolsToHarvest.Contains(item);
        }

        // maybe should be an inventory for return type???
        public List<ItemContainer> Harvest()
        {
            var result = new List<ItemContainer>();

            foreach (var resource in CanProduce)
            {
                var roll = Random.Range(0f, 1f);

                if (roll >= (1 - resource.rarity))
                {
                    var container = new ItemContainer(inventory:null, -1);
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

            return result;
        }
    }
}
