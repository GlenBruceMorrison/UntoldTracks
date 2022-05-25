using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.InventorySystem;

[CreateAssetMenu(fileName = "Item", menuName = "Data/InventorySeed")]
public class InventorySeed : ScriptableObject
{
    [System.Serializable]
    public class ItemContainerTemplate
    {
        public Item item;
        public int count;
    }
    
    [SerializeField] private List<ItemContainerTemplate> _initialContents = new List<ItemContainerTemplate>();

    public void Seed(Inventory inventory)
    {
        foreach (var containerTemplate in _initialContents)
        {
            if (containerTemplate.item == null || containerTemplate.count < 1)
            {
                continue;
            }

            inventory.Give(new ItemContainer(containerTemplate.item, containerTemplate.count));
        }
    }
}
