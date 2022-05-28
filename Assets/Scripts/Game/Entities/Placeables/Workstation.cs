using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;

public class Workstation : PlaceableEntity, IInteractable
{
    public string DisplayText { get; }
    public Sprite DisplaySprite { get; }
    public List<InteractionDisplay> PossibleInputs => new List<InteractionDisplay>()
    {
        new InteractionDisplay(InteractionInput.Primary, "Place")
    };

    public void HandleInput(PlayerManager manager, InteractionInput input)
    {
        throw new System.NotImplementedException();
    }

    public void HandleBecomeFocus(PlayerManager player)
    {
        throw new System.NotImplementedException();
    }

    public void HandleLoseFocus(PlayerManager player)
    {
        throw new System.NotImplementedException();
    }
}
