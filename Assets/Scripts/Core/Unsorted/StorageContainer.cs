using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UnityEngine;
using UnityEngine.Serialization;
using UntoldTracks;
using UntoldTracks.Player;
using UntoldTracks.Managers;

public class StorageContainer : PlaceableEntity, IInteractable
{
    public int size;
    
    private Inventory _inventory;
    [FormerlySerializedAs("_seed")] [SerializeField] private InventorySeed _inventorySeed;
    [SerializeField] private string _displayText;
    [SerializeField] private Sprite _displaySprite;

    public List<InteractionDisplay> PossibleInputs => new List<InteractionDisplay>()
    {
        new InteractionDisplay(InteractionInput.Action1, $"Open")
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

    private void Awake()
    {
        _inventory = new Inventory(size);
        if (_inventorySeed != null)
        {
            _inventorySeed.Seed(this._inventory);
        }
    }

    public void HandleInput(PlayerManager manager, InteractionInput input)
    {
        switch (input)
        {
            case InteractionInput.Action1:
                manager.PlayerManagerUI.OpenMainWindow();
                break;
        }
    }

    public void HandleBecomeFocus(PlayerManager player)
    {

    }


    public void HandleLoseFocus(PlayerManager player)
    {

    }
}
