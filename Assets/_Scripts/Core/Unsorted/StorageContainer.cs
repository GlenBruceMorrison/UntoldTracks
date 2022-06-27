using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UnityEngine;
using UnityEngine.Serialization;

using UntoldTracks;
using UntoldTracks.Player;
using UntoldTracks.Managers;
using UntoldTracks.Models;
using SimpleJSON;
using UnityEngine.Events;

public class StorageContainer : PlaceableEntity, IInteractable, ITokenizable
{
    public int size;
    
    private Inventory _inventory;

    [SerializeField] private string _displayText;
    [SerializeField] private Sprite _displaySprite;

    public Vector3 InteractionAnchor => transform.position;
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


    public event InteractionStateUpdate OnInteractionStateUpdate;

    public override void Start()
    {
        if (_inventory == null)
        {
            _inventory = new Inventory(8);
        }
    }

    public void HandleInput(PlayerManager manager, InteractionData interaction)
    {
        switch (interaction.Input)
        {
            case InteractionInput.Action1:
                manager.PlayerManagerUI.OpenMainWindow(_inventory);
                break;
        }
    }

    public void HandleBecomeFocus(PlayerManager player)
    {

    }


    public void HandleLoseFocus(PlayerManager player)
    {

    }

    public override void Load(JSONNode node)
    {
        base.Load(node);
        _inventory = new Inventory(node["inventory"]);
    }

    public override JSONObject Save()
    {
        var node = base.Save();

        node["inventory"] = _inventory.Save();

        return node;
    }
}
