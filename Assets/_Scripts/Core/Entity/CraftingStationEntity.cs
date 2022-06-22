using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Models;
using UntoldTracks.Managers;
using UnityEngine.Events;

namespace UntoldTracks
{
    public class CraftingStationEntity : PlaceableEntity, IInteractable
    {
        [Header("Recipes that this crafting station can produce")]
        public StationModel _data;
        public string DisplayText => _data._item.displayName;

        public List<InteractionDisplay> PossibleInputs => new()
        {
            new InteractionDisplay(InteractionInput.Action1, $"Work")
        };

        public Vector3 InteractionAnchor => transform.position + Vector3.up;


        public event InteractionStateUpdate OnInteractionStateUpdate;
        public void HandleBecomeFocus(PlayerManager player) { }
        public void HandleLoseFocus(PlayerManager player) { }

        public void HandleInput(PlayerManager manager, InteractionInput input)
        {
            if (input == InteractionInput.Action1)
            {
                manager.PlayerManagerUI.OpenMainWindow(null, _data.Recipes);
            }
        }
    }
}
