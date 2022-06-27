using System;
using System.Collections.Generic;
using UnityEngine;

using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Managers;
using UnityEngine.Events;

namespace UntoldTracks
{
    public delegate void InteractionStateUpdate();

    public interface IInteractable
    {
        /// <summary>
        /// The list of possible inputs from this interactable
        /// </summary>
        public List<InteractionDisplay> PossibleInputs { get; }

        /// <summary>
        /// This is where the 3D interactable canvas will be attatched to
        /// </summary>
        public Vector3 InteractionAnchor { get; }

        /// <summary>
        /// Let's the interaction controller know that this state has 
        /// been update and that the interaction canvas will have to
        /// be computed again.
        /// </summary>
        public event InteractionStateUpdate OnInteractionStateUpdate;

        /// <summary>
        /// This is called when the player clicks on this interactable
        /// </summary>
        /// <param name="manager">The PlayerMananger object that the interaction came from</param>
        /// <param name="input">The input type cliked when interacting</param>
        public void HandleInput(PlayerManager manager, InteractionData interaction);

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