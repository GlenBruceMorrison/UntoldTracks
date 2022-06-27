using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UnityEngine;
using UntoldTracks.Player;
using UntoldTracks;
using UntoldTracks.Managers;
using UnityEngine.Events;

public class ItemContainerWorldObject : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ItemContainer _container;
    public List<InteractionDisplay> _possibleInputs = new()
    {
        new InteractionDisplay(InteractionInput.Action1, $"Take")
    };

    public Vector3 InteractionAnchor => transform.position;
    public List<InteractionDisplay> PossibleInputs => _possibleInputs;


    public event InteractionStateUpdate OnInteractionStateUpdate;

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

    public void HandleInput(PlayerManager manager, InteractionData interaction)
    {
        if (interaction.Input == InteractionInput.Action1)
        {
            manager.InventoryController.Inventory.Give(Container);
            Destroy(this.gameObject);
        }
    }

    public void HandleLoseFocus(PlayerManager player)
    {

    }
}
