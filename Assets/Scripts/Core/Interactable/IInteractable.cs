using System;
using UnityEngine;
using UntoldTracks.Inventory;
using UntoldTracks.Player;

namespace UntoldTracks
{
    public enum InteractionInput
    {
        Primary,
        Secondary
    }

    public interface IInteractable
    {
        public string DisplayText { get; }
        public Sprite DisplaySprite { get; }

        public void HandlePrimaryInput(PlayerManager player, ItemContainer usingContainer);
        public void HandleSecondaryInput(PlayerManager player, ItemContainer usingContainer);
        public void HandleBecomeFocus(PlayerManager player);
        public void HandleLoseFocus(PlayerManager player);
    }
}