using System.Collections;
using System.Collections.Generic;
using Tracks.Inventory;
using UnityEngine;

public class ItemContainerWorldObject : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ItemContainer _container;
    public ItemContainer Container 
    {
        get
        {
            return _container;
        }
        set
        {
            _container.FillAndReturnRemaining(value.Item, value.Count);
        }
    }

    public void HandleInteraction(PlayerManager player)
    {
        var left = player.inventoryController.PlayerInventory.FillAndReturnRemaining(Container.Item, Container.Count);
        Destroy(this.gameObject);
    }

    public void HandleBecomeFocus(PlayerManager player)
    {

    }

    public void HandleLoseFocus(PlayerManager player)
    {

    }
}
