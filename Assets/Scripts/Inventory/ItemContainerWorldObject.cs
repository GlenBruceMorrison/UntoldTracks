using System.Collections;
using System.Collections.Generic;
using UntoldTracks.Inventory;
using UnityEngine;
using UntoldTracks.Player;
using UntoldTracks;

public class ItemContainerWorldObject : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ItemContainer _container;

    [SerializeField]
    private string _displayText;

    [SerializeField]
    private Sprite _displaySprite;

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

    public void HandleInteraction(PlayerManager player, ItemContainer usingContainer, InteractionInput input)
    {
        var left = player.inventoryController.Inventory.FillAndReturnRemaining(Container.Item, Container.Count);
        Destroy(this.gameObject);
    }

    public void HandleBecomeFocus(PlayerManager player)
    {

    }

    public void HandleLoseFocus(PlayerManager player)
    {

    }
}
