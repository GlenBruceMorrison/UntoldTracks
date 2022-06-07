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
    public class Kiln : PlaceableEntity, ITokenizable
    {
        [SerializeField] private float _fuelDegradeRate;
        [SerializeField] private List<Transform> _fuelModels = new();
        [SerializeField] private float _currentFuel;
        [SerializeField] private float _currentCookingTime;

        public UnityEvent OnFuelAdded, OnFuelEmptied, OnBurningStart, OnBurningStop;

        public ItemModel fuelModel;

        public KilnRecipe current;

        public List<KilnRecipe> recipes = new();

        public bool cooking = false;

        public int MaxFuel
        {
            get
            {
                return _fuelModels.Count;
            }
        }

        public float CurrentFuel
        {
            get
            {
                return _currentFuel;
            }
            set
            {
                _currentFuel = value;

                if (_currentFuel > MaxFuel)
                {
                    _currentFuel = MaxFuel;
                }

                for (var i = 0; i < MaxFuel; i++)
                {
                    _fuelModels[i].gameObject.SetActive(Mathf.FloorToInt(_currentFuel) > i);
                }
            }
        }

        public bool CanAddFuel(int amount=1)
        {
            return _currentFuel + amount < MaxFuel;
        }

        
        public void AddFuel(PlayerManager player)
        {
            if (!CanAddFuel(1))
            {
                return;
            }

            // Check if player has fuel needed in inventory
            if (!player.InventoryController.Inventory.CanTake(fuelModel, 1))
            {
                return;
            }

            // remove fuel from player and add fuel level
            player.InventoryController.Inventory.Take(new ItemQuery(fuelModel, 1));

            OnFuelAdded?.Invoke();

            CurrentFuel++;
        }

        public void Reset()
        {
            if (current != null)
            {
                current.Hide();
                current = null;
            }

            _currentCookingTime = 0;
            cooking = false;
        }

        public void TakeItem(PlayerManager player)
        {
            var item = current.output.model;

            if (player.InventoryController.Inventory.CanGive(item, 1))
            {
                player.InventoryController.Inventory.Give(new ItemContainer(item, 1));
            }

            Reset();
        }

        public void GiveItem(PlayerManager player)
        {
            // check if we are already processing something
            if (current != null)
            {
                if (current.IsCooked(_currentCookingTime))
                {
                    TakeItem(player);
                }

                return;
            }

            // item that the player is holder
            var item = player.InventoryController.ActiveItemContainer.Item;

            // try and find this item in our recipes
            foreach (var recipe in recipes)
            {
                if (recipe.input.model != item)
                {
                    continue;
                }

                current = recipe;
            }
            
            // cannot cook this item
            if (current == null)
            {
                return;
            }

            if (player.InventoryController.Inventory.CanTake(item, 1))
            {
                player.InventoryController.Inventory.Take(new ItemQuery(item, 1));
            }

            _currentCookingTime = 0;
            current.input.representation.gameObject.SetActive(true);
            cooking = true;
            OnBurningStart?.Invoke();
        }

        private void Update()
        {
            if (current == null)
            {
                return;
            }

            var isCooked = current.IsCooked(_currentCookingTime);

            if (isCooked)
            {
                if (cooking)
                {
                    cooking = false;
                    OnBurningStop?.Invoke();
                }
            }
            else
            {
                if (CurrentFuel > 0)
                {
                    if (!cooking)
                    {
                        cooking = true;
                        OnBurningStart?.Invoke();
                    }

                    CurrentFuel -= _fuelDegradeRate * Time.deltaTime;
                    _currentCookingTime += Time.deltaTime;
                }
                else
                {
                    if (cooking)
                    {
                        cooking = false;
                        OnBurningStop?.Invoke();
                    }
                }
            }
        }


        #region Token
        /*
        public override void Load(JSONNode node)
        {
            base.Load(node);
        }

        public override JSONObject Save()
        {
            var placeableJSON = new JSONObject();

            placeableJSON.Add("itemGUID", source.Guid);
            placeableJSON.Add("entity", base.Save());

            return placeableJSON;
        }
        */
        #endregion
    }
}
