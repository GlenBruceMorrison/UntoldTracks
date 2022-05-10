using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UntoldTracks.Player;

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

        public void HandleBecomeFocus(PlayerManager player)
        {
            onGainFocus?.Invoke(player);
        }

        public void HandleInteraction(PlayerManager player)
        {
            onInteract?.Invoke(player);
        }

        public void HandleLoseFocus(PlayerManager player)
        {
            onLoseFocus?.Invoke(player);
        }
    }
}
