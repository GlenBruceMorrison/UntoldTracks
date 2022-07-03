using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UntoldTracks.InventorySystem;
using UnityEngine.Events;
using UntoldTracks.Models;

namespace UntoldTracks.Resource
{
    [CreateAssetMenu(fileName = "Item", menuName = "Data/Resource")]
    public class ResourceHolder : ScriptableObject
    {
        public List<ResourceContainer> CanProduce = new List<ResourceContainer>();

        public UnityAction OnHarvested;

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
                    container.Give(new ItemContainer(resource.item, resource.baseAmount));

                    for (int i=0; i<resource.maxPossible; i++)
                    {
                        var multipleItemRoll = Random.Range(0f, 1f);

                        if (multipleItemRoll >= (1 - resource.multipleChance))
                        {
                            container.Give(new ItemContainer(resource.item, Random.Range(1, resource.baseAmount)));
                        }
                    }
                    
                    result.Add(container);
                }
            }

            return result;
        }
    }
}

