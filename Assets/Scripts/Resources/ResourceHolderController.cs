using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks.Player;

namespace UntoldTracks.Resource
{
    public class ResourceHolderController : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private string _displayText;

        [SerializeField]
        private Sprite _displaySprite;

        public string DisplayText
        {
            get
            {
                return _displayText;
            }
        }

        public Sprite DisplaySprite
        {
            get
            {
                return _displaySprite;
            }
        }

        public ResourceHolder holder;

        public UnityEvent OnHarvested;

        public int capacity = 10;

        public AudioSource source;


        public void HandleLoseFocus(PlayerManager player) { }
        public void HandleBecomeFocus(PlayerManager player) { }
        public void HandleInteraction(PlayerManager playerManager)
        {
            if (capacity < 1)
            {
                return;
            }

            var collected = holder.Harvest();

            if (collected == null)
            {
                return;
            }

            capacity -= 1;

            if (capacity < 1)
            {
                OnHarvested?.Invoke();
                //Destroy(this.gameObject);
                source.PlayOneShot(holder.fullyHarvestedAudio);
            }
            else
            {
                source.PlayOneShot(holder.harvestAudio);
            }

            foreach (var container in collected)
            {
                playerManager.inventoryController.Inventory.FillAndReturnRemaining(container.Item, container.Count);
            }
        }
    }
}
