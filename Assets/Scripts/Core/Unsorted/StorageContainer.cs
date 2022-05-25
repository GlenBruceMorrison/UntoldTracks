using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UnityEngine;

using UntoldTracks;
using UntoldTracks.Player;

public class StorageContainer : PlaceableEntity, IInteractable
{
    public int size;
    
    private Inventory _inventory;
    [SerializeField] private List<ItemContainerTemplate> _starterTemplate = new List<ItemContainerTemplate>();
    [SerializeField] private string _displayText;
    [SerializeField] private Sprite _displaySprite;

    public string DisplayText
    {
        get
        {
            return _displayText;
        }
    }

    public Sprite DisplaySprite
    {
        get
        {
            return _displaySprite;
        }
    }

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

            _inventory.Give(new ItemContainer(containerTemplate.item, containerTemplate.count));
        }
    }

    public void HandleBecomeFocus(PlayerManager player)
    {

    }


    public void HandleLoseFocus(PlayerManager player)
    {

    }

    public void HandlePrimaryInput(PlayerManager player, ItemContainer usingContainer)
    {

    }

    public void HandleSecondaryInput(PlayerManager player, ItemContainer usingContainer)
    {
        player.InventoryController.Open(_inventory);
    }
}
