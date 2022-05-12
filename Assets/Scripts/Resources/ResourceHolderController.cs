using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks.Inventory;
using UntoldTracks.Player;

namespace UntoldTracks.Resource
{
    public class ResourceHolderController : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private string _displayText;

        [SerializeField]
        private Sprite _displaySprite;

        public bool deleteOnHarvested;

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
        public void HandleInteraction(PlayerManager playerManager, ItemContainer usingContainer, InteractionInput input)
        {
            if (capacity < 1)
            {
                return;
            }

            if (holder.itemToHarvest != null)
            {
                if (!playerManager.inventoryController.HasActiveItem)
                {
                    return;
                }

                if (playerManager.inventoryController.ActiveItem.Item != holder.itemToHarvest)
                {
                    return;
                }
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

                if (deleteOnHarvested)
                {
                    Destroy(this.gameObject);
                }

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
