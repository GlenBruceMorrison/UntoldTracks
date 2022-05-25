using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UntoldTracks.Player;
using UntoldTracks.InventorySystem;

namespace UntoldTracks
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _displayText;
        [SerializeField] private Sprite _displaySprite;

        public string DisplayText => _displayText;
        public Sprite DisplaySprite => _displaySprite;

        public UnityEvent<PlayerManager> onPrimaryInteract, onSecondaryInteract, onLoseFocus, onGainFocus;

        public virtual void HandleBecomeFocus(PlayerManager player)
        {
            onGainFocus?.Invoke(player);
        }

        public void HandleLoseFocus(PlayerManager player)
        {
            onLoseFocus?.Invoke(player);
        }

        public void HandlePrimaryInput(PlayerManager player, ItemContainer usingContainer)
        {
            onPrimaryInteract?.Invoke(player);
        }

        public void HandleSecondaryInput(PlayerManager player, ItemContainer usingContainer)
        {
            onSecondaryInteract?.Invoke(player);
        }
    }
}
