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

namespace UntoldTracks.Machines
{
    [System.Serializable]
    public class KilnInput
    {
        public ItemModel itemInput;
        public GameObject inputItemPrefab;

        public ItemModel itemOutput;
        public GameObject outputItemPrefab;

        public GameObject coolingItemPrefab;

        public float fuelRequired;
        public float currentBurnTime;

        [HideInInspector] public bool readyToTake = false;
    }

    public class KilnView : PlaceableEntity
    {
        [SerializeField] private float _fuelDegradeRate;
        [SerializeField] private List<Transform> _fuelModels = new List<Transform>();
        
        public ItemModel fuel;
        public Transform cookingItemContainer, giveItemTransform, takeItemTransform, cookingItemTransform;

        private KilnInput cookingItem;

        private float _currentFuelLevel;

        private bool _burning = false;
        
        public UnityEvent OnBurning, OnStop;
        [NonReorderable] public List<KilnInput> PossibleInputs;

        public KilnState currentState;
        public enum KilnState
        {
            Idle,
            Cooking,
            Cooling,
            Cooked
        }

        public bool HasItem
        {
            get
            {
                return cookingItem != null;
            }
        }

        public bool IsItemReady
        {
            get
            {
                return cookingItem.currentBurnTime > cookingItem.fuelRequired;
            }
        }

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
                }

                if (_burning && _currentFuelLevel <= 0)
                {
                    _burning = false;
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
            var hasFuel = player.InventoryController.Inventory.CanTake(fuel, 1);
            
            if (!hasFuel)
            {
                return;
            }
            
            // remove fuel from player and add fuel level
            player.InventoryController.Inventory.Take(new ItemQuery(fuel, 1));
            
            CurrentFuelLevel += 1;
        }

        public void TakeItem(PlayerManager player)
        {
            if (currentState != KilnState.Cooked)
            {
                return;
            }

            player.InventoryController.Inventory.Give(new ItemContainer(cookingItem.itemOutput, 1));

            ToIdle();
        }

        public void GiveItem(PlayerManager player)
        {
            if (currentState != KilnState.Idle)
            {
                return;
            }

            var item = player.InventoryController.ActiveItemContainer.Item;

            foreach (var kilnItem in PossibleInputs)
            {
                if (kilnItem.itemInput == item)
                {
                    cookingItem = kilnItem;
                    break;
                }
            }

            if (cookingItem == null)
            {
                return;
            }

            player.InventoryController.Inventory.Take(new ItemQuery(cookingItem.itemInput));

            ToCooking();
        }

        private void ToCooking()
        {
            giveItemTransform.gameObject.SetActive(false);
            takeItemTransform.gameObject.SetActive(false);

            cookingItem.currentBurnTime = 0.0f;

            var prefab = Instantiate(cookingItem.inputItemPrefab, cookingItemContainer.transform.position, Quaternion.identity);
            prefab.transform.parent = cookingItemContainer;
            prefab.transform.localPosition = Vector3.zero;
            cookingItemTransform = prefab.transform;

            Debug.Log(prefab);
            currentState = KilnState.Cooking;

            OnBurning?.Invoke();
        }

        private void ToCooling()
        {
            giveItemTransform.gameObject.SetActive(false);
            takeItemTransform.gameObject.SetActive(false);

            Destroy(cookingItemTransform.gameObject);

            var prefab = Instantiate(cookingItem.coolingItemPrefab, cookingItemContainer.transform.position, Quaternion.identity);
            prefab.transform.parent = cookingItemContainer;
            prefab.transform.localPosition = Vector3.zero;
            cookingItemTransform = prefab.transform;

            OnStop?.Invoke();
        }

        private void ToCooked()
        {
            Destroy(cookingItemTransform.gameObject);

            var prefab = Instantiate(cookingItem.outputItemPrefab);
            prefab.transform.parent = cookingItemContainer;
            prefab.transform.localPosition = Vector3.zero;
            cookingItemTransform = prefab.transform;

            giveItemTransform.gameObject.SetActive(false);
            takeItemTransform.gameObject.SetActive(true);
        }

        private void ToIdle()
        {
            Destroy(cookingItemTransform.gameObject);

            cookingItem = null;

            giveItemTransform.gameObject.SetActive(true);
            takeItemTransform.gameObject.SetActive(false);
        }

        private void Update()
        {
            switch (currentState)
            {
                case KilnState.Cooking:
                    CurrentFuelLevel -= _fuelDegradeRate * Time.deltaTime;
                    cookingItem.currentBurnTime += _fuelDegradeRate * Time.deltaTime;
                    Debug.Log(cookingItem.currentBurnTime);

                    if (cookingItem.currentBurnTime > cookingItem.fuelRequired)
                    {
                        ToCooling();
                        currentState = KilnState.Cooling;
                    }
                    break;
                case KilnState.Cooked:

                    break;
                case KilnState.Idle:

                    break;
                case KilnState.Cooling:
                    cookingItem.currentBurnTime -= _fuelDegradeRate * Time.deltaTime;
                    if (cookingItem.currentBurnTime <= 0)
                    {
                        ToCooked();
                        currentState = KilnState.Cooked;
                    }
                    break;
            }
        }
    }
}
