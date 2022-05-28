using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UntoldTracks.Player;
using UntoldTracks.InventorySystem;
using System.Collections.Generic;

namespace UntoldTracks
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _displayText;
        [SerializeField] private Sprite _displaySprite;
        [SerializeField] private List<InteractionDisplay> _displayList;

        public string DisplayText => _displayText;
        public Sprite DisplaySprite => _displaySprite;
        public List<InteractionDisplay> PossibleInputs => _displayList;

        public UnityEvent<PlayerManager, InteractionInput> onInteraction;
        public UnityEvent<PlayerManager> onLoseFocus, onGainFocus;


        public void HandleInput(PlayerManager manager, InteractionInput input)
        {
            onInteraction?.Invoke(manager, input);
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
