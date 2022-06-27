using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Managers;

namespace UntoldTracks.Resource
{
    public class ResourceHolderController : MonoBehaviour, IInteractable
    {
        [SerializeField]  private string _displayText;

        [SerializeField] private Sprite _displaySprite;

        public int durability;
        //public int capacity = 10;

        public Vector3 InteractionAnchor => transform.position;


        public event InteractionStateUpdate OnInteractionStateUpdate;
        public List<InteractionDisplay> PossibleInputs => new List<InteractionDisplay>()
        {
            new InteractionDisplay(InteractionInput.Primary, $"Harvest {holder.name}.")
        };

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


        public ResourceHolder holder;

        public UnityEvent OnHarvested;

        public AudioSource source;


        public void HandleLoseFocus(PlayerManager player) { }
        public void HandleBecomeFocus(PlayerManager player) { }

        public void HandleInput(PlayerManager manager, InteractionData interaction)
        {
            if (interaction.Input == InteractionInput.Primary)
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

                var container = manager.InventoryController.ActiveItemContainer;

                if (!holder.IsItemValid(container.Item))
                {
                    return;
                }

                if (source != null && holder.harvestAudio != null)
                {
                    source.PlayOneShot(holder.harvestAudio);
                }

                durability -= 1;// container.Item.toolStrength;

                if (durability > 0)
                {
                    return;
                }

                var collected = holder.Harvest();

                if (collected == null)
                {
                    return;
                }

                foreach (var result in collected)
                {
                    manager.InventoryController.Inventory.Give(result);
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
}
