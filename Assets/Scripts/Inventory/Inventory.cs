using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public List<ItemContainer> containers = new List<ItemContainer>();

    public string name = "Inventory";

    [HideInInspector]
    public UnityEvent onModified;

    public List<ItemContainer> TransferToAndReturnRemaining(Inventory targetInventory)
    {
        foreach(var container in containers)
        {
            var itemToTransfer = container.item;
            var amountToTransfer = container.count;

            var remaining = RemoveAndReturnRemaining(container.item, container.count);
            var addedToDestination = targetInventory.AddAndReturnRemaining(itemToTransfer, amountToTransfer - remaining);
            if (addedToDestination > 0)
            {
                AddAndReturnRemaining(itemToTransfer, addedToDestination);
            }
        }

        return null;
    }

    public int AddAndReturnRemaining(Item itemToAdd, int toAdd)
    {
        var remaining = toAdd;

        containers.Where(x => x.item == itemToAdd).ToList().ForEach(x =>
        {
            if (remaining > 0)
            {
                remaining = x.AddAndReturnDifference(remaining);
            }
        });

        if (remaining > 0)
        {
            containers.Where(x => x.item == null).ToList().ForEach(x =>
            {
                if (remaining > 0)
                {
                    remaining = x.AddAndReturnDifference(itemToAdd, remaining);
                }
            });
        }

        onModified.Invoke();
        return remaining;
    }

    public int RemoveAndReturnRemaining(Item item, int toRemove)
    {
        var remaining = toRemove;

        containers.Where(x => x.item == item).Reverse().ToList().ForEach(x => {
            if (remaining > 0)
            {
                remaining = x.RemoveAndReturnRemaining(remaining);
            }
        });

        onModified.Invoke();
        return remaining;
    }

    public void RemoveAtSlot(int slotIndex)
    {
        containers[slotIndex].item = null;
        containers[slotIndex].count = 0;
        onModified.Invoke();
    }
}
