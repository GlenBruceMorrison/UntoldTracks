using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks.Player;
using UntoldTracks.Models;
using UntoldTracks.Managers;
using SimpleJSON;

namespace UntoldTracks.Machines
{
    public class KilnInputInteractable : MonoBehaviour, IInteractable
    {
        private List<InteractionDisplay> _possibleInputs;

        private Kiln _kiln;

        public List<InteractionDisplay> PossibleInputs => _possibleInputs;
        public Vector3 InteractionAnchor => transform.position;


        public event InteractionStateUpdate OnInteractionStateUpdate;

        private void Awake()
        {
            _kiln = GetComponentInParent<Kiln>();
        }

        private void OnEnable()
        {
            _kiln.OnItemAdded += UpdateInteractionState;
            _kiln.OnItemTaken += UpdateInteractionState;
            _kiln.OnItemCooked += UpdateInteractionState;
        }

        private void OnDisable()
        {
            _kiln.OnItemAdded -= UpdateInteractionState;
            _kiln.OnItemTaken -= UpdateInteractionState;
            _kiln.OnItemCooked -= UpdateInteractionState;
        }

        public void UpdateInteractionState()
        {
            if (_kiln.current == null)
            {
                var input = _kiln.recipes.FirstOrDefault(x =>
                    x.input.model == GameManager.Instance.LocalPlayer.InventoryController.ActiveItemContainer.Item);

                if (input != null)
                {
                    _possibleInputs = new List<InteractionDisplay>()
                    {
                        new InteractionDisplay(InteractionInput.Action1, "Add Item")
                    };

                    OnInteractionStateUpdate?.Invoke();
                    return;
                }
            }
            else
            {
                if (_kiln.HoldingCookedItem)
                {
                    _possibleInputs = new List<InteractionDisplay>()    
                    {
                        new InteractionDisplay(InteractionInput.Action1, $"Take {_kiln.current.output.model.displayName}")
                    };

                    OnInteractionStateUpdate?.Invoke();
                    return;
                }
            }

            _possibleInputs = new();
            OnInteractionStateUpdate?.Invoke();
        }

        public void HandleBecomeFocus(PlayerManager player)
        {
            UpdateInteractionState();
        }

        public void HandleInput(PlayerManager manager, InteractionData interaction)
        {
            if (interaction.Input != InteractionInput.Action1) return;
            _kiln.GiveItem(manager);
        }

        public void HandleLoseFocus(PlayerManager player)
        {

        }
    }
}
