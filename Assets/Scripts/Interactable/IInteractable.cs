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

        public void HandleInteraction(PlayerManager player, ItemContainer usingContainer, InteractionInput input);
        public void HandleBecomeFocus(PlayerManager player);
        public void HandleLoseFocus(PlayerManager player);
    }
}