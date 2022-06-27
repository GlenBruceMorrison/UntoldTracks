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
    public class KilnFuelInteractable : MonoBehaviour, IInteractable
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
            _kiln.OnFuelAdded.AddListener(() => UpdateInteractionState());
            _kiln.OnFuelBurned += () => UpdateInteractionState();
        }

        private void OnDisable()
        {
            _kiln.OnFuelAdded.RemoveAllListeners();
            _kiln.OnFuelBurned -= () => UpdateInteractionState();
        }

        public void UpdateInteractionState()
        {
            if (_kiln.CurrentFuel +2 < _kiln.MaxFuel)
            {
                _possibleInputs = new List<InteractionDisplay>()
                {
                    new InteractionDisplay(InteractionInput.Action1, "Add Fuel")
                };

                OnInteractionStateUpdate?.Invoke();
                return;
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
            _kiln.AddFuel(manager);
        }

        public void HandleLoseFocus(PlayerManager player)
        {

        }
    }
}
