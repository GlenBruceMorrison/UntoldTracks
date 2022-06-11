using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Models;
using UntoldTracks.Managers;

namespace UntoldTracks
{
    public class CraftingStationEntity : PlaceableEntity, IInteractable
    {
        public StationModel _data;
        public string DisplayText => _data._item.displayName;
        public Sprite DisplaySprite => _data._item.sprite;

        public List<InteractionDisplay> PossibleInputs => new List<InteractionDisplay>()
        {
            new InteractionDisplay(InteractionInput.Action1, $"Work")
        };


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
