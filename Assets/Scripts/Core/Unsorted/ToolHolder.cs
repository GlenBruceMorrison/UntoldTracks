using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.Inventory;
using UntoldTracks.Player;

public class ToolHolder : MonoBehaviour, IInteractable
{
    private Item _holding;
    private GameObject _holdiningWorldObject;

    public Sprite emptyIcon;

    public string DisplayText
    {
        get
        {
            if (_holding != null)
            {
                return $"Take {_holding.name}";
            }

            return "Holder";
        }
    }

    public Sprite DisplaySprite
    {
        get
        {
            if (_holding != null)
            {
                return _holding.sprite;
            }

            return emptyIcon;
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
        // if we are already holding something we want to remove it
        if (_holdiningWorldObject != null)
        {
            Destroy(_holdiningWorldObject);
            player.inventoryController.Inventory.Give(usingContainer);
        }
    }

    public void HandleSecondaryInput(PlayerManager player, ItemContainer usingContainer)
    {
        // check if we can actually store this or not
        if (usingContainer.Item.toolPrefab)
        {
            if (_holdiningWorldObject != null)
            {
                return;
            }

            // add a reference to what item we are holding
            _holding = usingContainer.Item;

            // get the world representation of this item and spawn it in to the scene
            _holdiningWorldObject = Instantiate(usingContainer.Item.toolPrefab, transform);

            // remove this item from the players inventory
            player.inventoryController.Inventory.Take(new ItemQuery(usingContainer.Item, usingContainer.Count));
        }
    }
}
