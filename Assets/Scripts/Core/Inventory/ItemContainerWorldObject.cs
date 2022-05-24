using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
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
            _container.Give(value);
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
        player.InventoryController.Inventory.Give(Container);
        Destroy(this.gameObject);
    }

    public void HandleSecondaryInput(PlayerManager player, ItemContainer usingContainer)
    {


    }
}
