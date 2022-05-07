using System.Collections;
using System.Collections.Generic;
using Tracks.Inventory;
using UnityEngine;

public class StorageContainer : MonoBehaviour, IInteractable
{
    public IInventory _inventory;

    public int size;

    [SerializeField]
    private List<ItemContainerTemplate> _starterTemplate = new List<ItemContainerTemplate>();

    private void Awake()
    {
        _inventory = new Inventory(size);
        if (_starterTemplate != null)
        {
            AddFromTempalte();
        }
    }

    public void AddFromTempalte()
    {
        foreach (var containerTemplate in _starterTemplate)
        {
            if (containerTemplate.item == null || containerTemplate.count < 1)
            {
                continue;
            }

            _inventory.FillAndReturnRemaining(containerTemplate.item, containerTemplate.count);
        }
    }

    public void HandleBecomeFocus(PlayerManager player)
    {

    }

    public void HandleInteraction(PlayerManager player)
    {
        player.inventoryController.OpenOtherInventory(_inventory);
    }

    public void HandleLoseFocus(PlayerManager player)
    {

    }
}
