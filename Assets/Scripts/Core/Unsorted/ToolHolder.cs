using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;

public class ToolHolder : MonoBehaviour, IInteractable
{
    private Item _holding;
    private GameObject _holdiningWorldObject;

    public Sprite emptyIcon;
    public List<InteractionDisplay> PossibleInputs => new List<InteractionDisplay>()
    {
        new InteractionDisplay(InteractionInput.Secondary, _holding == null ? "Place" : "Take")
    };

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
    }

    public void HandleSecondaryInput(PlayerManager player, ItemContainer usingContainer)
    {
    }

    public void HandleInput(PlayerManager manager, InteractionInput input)
    {
        switch (input)
        {
            case InteractionInput.Primary:
                // if we are already holding something we want to remove it
                if (_holdiningWorldObject != null)
                {
                    Destroy(_holdiningWorldObject);
                    manager.InventoryController.Inventory.Give(manager.InventoryController.ActiveItem);
                }
                break;
            case InteractionInput.Secondary:
                var container = manager.InventoryController.ActiveItem;

                // check if we can actually store this or not
                if (container.Item.toolPrefab)
                {
                    if (_holdiningWorldObject != null)
                    {
                        return;
                    }

                    // add a reference to what item we are holding
                    _holding = container.Item;

                    // get the world representation of this item and spawn it in to the scene
                    _holdiningWorldObject = Instantiate(container.Item.toolPrefab.gameObject, transform);

                    // remove this item from the players inventory
                    manager.InventoryController.Inventory.Take(new ItemQuery(container.Item, container.Count));
                }
                break;
        }
    }
}
