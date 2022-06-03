using System;
using System.Collections.Generic;
using UnityEngine;

using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Managers;

namespace UntoldTracks
{

    public interface IInteractable
    {
        /// <summary>
        /// The sprite that shows when the player focusses on this interactable
        /// </summary>
        public Sprite DisplaySprite { get; }

        /// <summary>
        /// The list of possible inputs from this interactable
        /// </summary>
        public List<InteractionDisplay> PossibleInputs { get; }

        /// <summary>
        /// This is called when the player clicks on this interactable
        /// </summary>
        /// <param name="manager">The PlayerMananger object that the interaction came from</param>
        /// <param name="input">The input type cliked when interacting</param>
        public void HandleInput(PlayerManager manager, InteractionInput input);

        /// <summary>
        /// Called when the player looks at this interactable
        /// </summary>
        /// <param name="player">The PlayerMananger object that the interaction came from</param>
        public void HandleBecomeFocus(PlayerManager player);

        /// <summary>
        /// Called when the player was looking at this interactable and then looked away
        /// </summary>
        /// <param name="player">The PlayerMananger object that the interaction came from</param>
        public void HandleLoseFocus(PlayerManager player);
    }
}