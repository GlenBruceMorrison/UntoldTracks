using System;
using System.Collections;
using System.Collections.Generic;
using UntoldTracks.Inventory;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks.Player;

namespace UntoldTracks.Machines
{
    public class Kiln : PlaceableEntity
    {
        [SerializeField]
        private float _fuelDegradeRate;
        
        [SerializeField]
        private List<Transform> _fuelModels = new List<Transform>();
        
        public Item fuel;

        private float _currentFuelLevel;

        private bool _burning = false;
        
        public UnityEvent OnBurning, OnStop;
        
        public int MaxFuel
        {
            get
            {
                return _fuelModels.Count;
            }
        }
        
        public float CurrentFuelLevel
        {
            get
            {
                return _currentFuelLevel;
            }
            set
            {
                _currentFuelLevel = value;
                
                for (var i=0; i<MaxFuel; i++)
                {
                    _fuelModels[i].gameObject.SetActive(Mathf.FloorToInt(_currentFuelLevel) > i);
                }

                if (!_burning && _currentFuelLevel > 0)
                {
                    _burning = true;
                    OnBurning?.Invoke();
                }

                if (_burning && _currentFuelLevel <= 0)
                {
                    _burning = false;
                    OnStop?.Invoke();
                }
            }
        }
        
        public void AddFuel(PlayerManager player)
        {
            // if already full then return
            if (CurrentFuelLevel >= MaxFuel)
            {
                return;
            }

            // Check if player has fuel needed in inventory
            var hasFuel = player.inventoryController.Inventory.HasItem(fuel, 1);
            
            if (!hasFuel)
            {
                return;
            }
            
            // remove fuel from player and add fuel level
            player.inventoryController.Inventory.TakeAndReturnRemaining(fuel, 1);
            
            CurrentFuelLevel += 1;
        }

        private void Update()
        {
            if (CurrentFuelLevel > 0)
            {
                CurrentFuelLevel -= _fuelDegradeRate * Time.deltaTime;
            }
        }
    }
}
