using System;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;

namespace UntoldTracks
{
    public enum InteractionInput
    {
        Primary,
        Secondary
    }

    [System.Serializable]
    public class InteractionDisplay
    {
        public InteractionInput input;
        public string text;

        public InteractionDisplay(InteractionInput input, string text)
        {
            this.input = input;
            this.text = text;
        }
    }

    public interface IInteractable
    {
        public Sprite DisplaySprite { get; }
        public List<InteractionDisplay> PossibleInputs { get; }

        public void HandleInput(PlayerManager manager, InteractionInput input);
        public void HandleBecomeFocus(PlayerManager player);
        public void HandleLoseFocus(PlayerManager player);
    }
}