using System;
using UnityEngine;
using UntoldTracks.Player;

namespace UntoldTracks
{
    public interface IInteractable
    {
        public string DisplayText { get; }
        public Sprite DisplaySprite { get; }

        public void HandleInteraction(PlayerManager player);
        public void HandleBecomeFocus(PlayerManager player);
        public void HandleLoseFocus(PlayerManager player);
    }
}