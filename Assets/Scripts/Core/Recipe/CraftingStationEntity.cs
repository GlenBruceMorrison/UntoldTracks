using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;

namespace UntoldTracks
{
    public class CraftingStationEntity : PlaceableEntity, IInteractable
    {
        public CraftingStationData _data;
        public string DisplayText => _data._item.name;
        public Sprite DisplaySprite => _data._item.sprite;
        
        public void HandlePrimaryInput(PlayerManager player, ItemContainer usingContainer)
        {
            player.PlayerManagerUI.OpenMainWindow(null, _data._recipeBook);
        }

        public void HandleSecondaryInput(PlayerManager player, ItemContainer usingContainer) { }
        public void HandleBecomeFocus(PlayerManager player) { }
        public void HandleLoseFocus(PlayerManager player) { }
    }
}
