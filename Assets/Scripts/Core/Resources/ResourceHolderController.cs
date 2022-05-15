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

        public int durability;
        //public int capacity = 10;

        public bool IsDepleted
        {
            get
            {
                return durability > 0;
            }
        }

        public bool deleteOnHarvested;

        public string DisplayText
        {
            get
            {
                return IsDepleted ? "" : _displayText;
            }
        }

        public Sprite DisplaySprite
        {
            get
            {
                return IsDepleted ? null : _displaySprite;
            }
        }

        public ResourceHolder holder;

        public UnityEvent OnHarvested;

        public AudioSource source;


        public void HandleLoseFocus(PlayerManager player) { }
        public void HandleBecomeFocus(PlayerManager player) { }
        public void HandleInteraction(PlayerManager playerManager, ItemContainer usingContainer, InteractionInput input)
        {
            /*
            if (capacity < 1)
            {
                return;
            }
            */

            /*
            if (holder.IsItemValid(usingContainer.Item))
            {
                if (!playerManager.inventoryController.HasActiveItem)
                {
                    return;
                }
            }
            */

            if (!holder.IsItemValid(usingContainer.Item))
            {
                return;
            }

            source.PlayOneShot(holder.harvestAudio);
            durability -= usingContainer.Item.toolStrength;

            if (durability > 0)
            {
                return;
            }

            var collected = holder.Harvest();

            if (collected == null)
            {
                return;
            }

            foreach (var container in collected)
            {
                playerManager.inventoryController.Inventory.FillAndReturnRemaining(container.Item, container.Count);
            }

            OnHarvested?.Invoke();

            if (deleteOnHarvested)
            {
                Destroy(this.gameObject);
            }

            source.PlayOneShot(holder.fullyHarvestedAudio);
        }
    }
}
