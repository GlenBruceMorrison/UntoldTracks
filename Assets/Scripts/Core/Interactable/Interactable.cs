using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UntoldTracks.Player;
using UntoldTracks.Inventory;

namespace UntoldTracks
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private string _displayText;

        [SerializeField]
        private Sprite _displaySprite;

        public string DisplayText => _displayText;
        public Sprite DisplaySprite => _displaySprite;

        public UnityEvent<PlayerManager> onInteract, onLoseFocus, onGainFocus;

        public Item Shovel;

        public virtual void HandleBecomeFocus(PlayerManager player)
        {
            onGainFocus?.Invoke(player);
        }

        public void HandleInteraction(PlayerManager player, ItemContainer usingContainer, InteractionInput input)
        {
            onInteract?.Invoke(player);
        }

        public void HandleLoseFocus(PlayerManager player)
        {
            onLoseFocus?.Invoke(player);
        }
    }
}
