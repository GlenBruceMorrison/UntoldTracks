using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UntoldTracks.Player;
using UntoldTracks.InventorySystem;
using System.Collections.Generic;
using UntoldTracks.Managers;

namespace UntoldTracks
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _displayText;
        [SerializeField] private Sprite _displaySprite;
        [SerializeField] private List<InteractionDisplay> _inputDisplay;

        public string DisplayText => _displayText;
        public Sprite DisplaySprite => _displaySprite;
        public List<InteractionDisplay> PossibleInputs => _inputDisplay;

        public UnityEvent<PlayerManager, InteractionInput> onInteraction;
        public UnityEvent<PlayerManager> onPrimaryInput, onSecondaryInput, onLoseFocus, onGainFocus;


        public void HandleInput(PlayerManager manager, InteractionInput input)
        {
            switch (input)
            {
                case InteractionInput.Primary:
                    onPrimaryInput?.Invoke(manager);
                    break;
                case InteractionInput.Secondary:
                    onSecondaryInput?.Invoke(manager);
                    break;
            }
        }

        public virtual void HandleBecomeFocus(PlayerManager player)
        {
            onGainFocus?.Invoke(player);
        }

        public void HandleLoseFocus(PlayerManager player)
        {
            onLoseFocus?.Invoke(player);
        }
    }
}
