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


        private void OnEnable()
        {
            holder.OnHarvested += () =>
            {
                OnHarvested?.Invoke();
                Destroy(this.gameObject);
            };
        }

        public void HandleLoseFocus(PlayerManager player) { }
        public void HandleBecomeFocus(PlayerManager player) { }
        public void HandleInteraction(PlayerManager playerManager)
        {
            var collected = holder.Harvest();

            if (collected == null)
            {
                return;
            }

            foreach (var container in collected)
            {
                playerManager.inventoryController.Inventory.FillAndReturnRemaining(container.Item, container.Count);
            }
        }
    }
}
