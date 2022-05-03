using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ItemContainer
{
    public Item item;
    public int count;

    [HideInInspector]
    public bool locked = false;

    public void Empty()
    {
        this.item = null;
        this.count = 0;
    }

#region add
    public void Add(int toAdd)
    {
        if (locked)
        {
            return;
        }

        if (toAdd < 1)
        {
            throw new ArgumentException("Must be more than 0");
        }

        if (item == null)
        {
            throw new Exception("This container has no item");
        }

        if (count + toAdd >= item.stackSize)
        {
            throw new Exception("Can't hold anymore stacks");
        }

        count += toAdd;
    }

    public int AddAndReturnDifference(int toAdd)
    {
        if (locked)
        {
            return toAdd;
        }

        if (item == null)
        {
            throw new Exception("This container has no item");
        }

        return AddAndReturnDifference(item, toAdd);
    }

    public int AddAndReturnDifference(Item itemToAdd, int toAdd)
    {
        if (locked)
        {
            return toAdd;
        }

        if (item != null && itemToAdd != item)
        {
            throw new ArgumentException($"This container contains {item}, trying to add {itemToAdd}");
        }

        if (toAdd < 1)
        {
            throw new ArgumentException("Must be more than 0");
        }

        if (item == null)
        {
            item = itemToAdd;
        }

        var diff = item.stackSize - (count + toAdd);

        if (diff < 0)
        {
            count = item.stackSize;
            return -diff;
        }

        count += toAdd;
        return 0;
    }
#endregion

#region remove
    public int RemoveAndReturnRemaining(int toRemove)
    {
        if (locked)
        {
            return toRemove;
        }

        if (item == null)
        {
            throw new Exception("Trying to take items from an empty container");
        }

        var diff = count - toRemove;

        if (diff < 0)
        {
            count = 0;
            item = null;

            return -diff;
        }

        count -= toRemove;

        if (count == 0)
        {
            item = null;
        }

        return 0;
    }
#endregion

#region queries
    public bool HasEnough(Item itemToAdd, int toAdd)
    {
        if (item != itemToAdd)
        {
            throw new ArgumentException($"Item mismatch, checking for {itemToAdd.name} but contains {item.name}");
        }

        return count >= toAdd;
    }
#endregion
}
