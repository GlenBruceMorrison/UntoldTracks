using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainerWorldObject : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ItemContainer _container = new ItemContainer();
    public ItemContainer Container 
    {
        get
        {
            return _container;
        }
        set
        {
            _container.item = value.item;
            _container.count = value.count;
        }
    }

    public void SetItemContainer(ItemContainer newContainer)
    {
        Container = newContainer;
    }

    public ItemContainer GetContainerValue()
    {
        return _container;
    }

    public void HandleInteraction(PlayerManager player)
    {
        player.inventoryController.HandleItemPickupFromWorld(this);
    }
}
