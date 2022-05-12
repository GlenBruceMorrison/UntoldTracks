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

    public void HandleInteraction(PlayerManager player, ItemContainer usingContainer, InteractionInput input)
    {
        // right click will remove something if it's stored
        if (input == InteractionInput.Primary)
        {
            // if we are already holding something we want to remove it
            if (_holdiningWorldObject != null)
            {
                Destroy(_holdiningWorldObject);
                player.inventoryController.Inventory.FillAndReturnRemaining(usingContainer.Item, usingContainer.Count);
            }
        }

        // left click will attempt to store the item the player is holding
        else if(input == InteractionInput.Secondary)
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
                player.inventoryController.Inventory.TakeAndReturnRemaining(usingContainer.Item, usingContainer.Count);
            }
        }
    }

    public void HandleLoseFocus(PlayerManager player)
    {

    }
}
