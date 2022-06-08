using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UnityEngine;
using UntoldTracks.Player;
using UntoldTracks;
using UntoldTracks.Managers;

public class ItemContainerWorldObject : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ItemContainer _container;

    [SerializeField]
    private string _displayText;

    [SerializeField]
    private Sprite _displaySprite;

    public List<InteractionDisplay> PossibleInputs => new List<InteractionDisplay>()
    {
        new InteractionDisplay(InteractionInput.Action1, $"Take")
    };

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

    public void HandleInput(PlayerManager manager, InteractionInput input)
    {
        Debug.Log(input);
        if (input == InteractionInput.Action1)
        {
            manager.InventoryController.Inventory.Give(Container);
            Destroy(this.gameObject);
        }
    }

    public void HandleLoseFocus(PlayerManager player)
    {

    }
}
