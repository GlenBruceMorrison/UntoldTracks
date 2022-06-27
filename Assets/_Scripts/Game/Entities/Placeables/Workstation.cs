using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Managers;
using UnityEngine.Events;

public class Workstation : PlaceableEntity, IInteractable
{
    public string DisplayText { get; }
    public List<InteractionDisplay> PossibleInputs => new List<InteractionDisplay>()
    {
        new InteractionDisplay(InteractionInput.Primary, "Place")
    };

    public Vector3 InteractionAnchor => transform.position + Vector3.up;


    public event InteractionStateUpdate OnInteractionStateUpdate;

    public void HandleInput(PlayerManager manager, InteractionData interaction)
    {

    }

    public void HandleBecomeFocus(PlayerManager player)
    {

    }

    public void HandleLoseFocus(PlayerManager player)
    {

    }
}
