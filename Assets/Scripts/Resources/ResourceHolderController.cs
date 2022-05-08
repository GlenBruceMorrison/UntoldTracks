using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tracks.Resource
{
    public class ResourceHolderController : MonoBehaviour, IInteractable
    {
        public ResourceHolder holder;

        public UnityEvent OnHarvested;

        public int capacity = 10;

        public AudioSource source;


        public void HandleLoseFocus(PlayerManager player) { }
        public void HandleBecomeFocus(PlayerManager player) { }
        public void HandleInteraction(PlayerManager playerManager)
        {
            var collected = holder.Harvest();

            if (collected == null)
            {
                return;
            }

            capacity -= 1;

            if (capacity < 1)
            {
                OnHarvested?.Invoke();
                Destroy(this.gameObject);
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
