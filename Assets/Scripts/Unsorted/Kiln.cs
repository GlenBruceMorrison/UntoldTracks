using System.Collections;
using System.Collections.Generic;
using Tracks.Inventory;
using UnityEngine;

namespace Tracks.Machines
{
    public class Kiln : MonoBehaviour
    {
        public int currentFuel = 0;
        public int maxFuel = 6;
        public List<Transform> fuelModels = new List<Transform>();
        public Item fuel;

        public void AddFuel(PlayerManager player)
        {
            // if already full then return
            if (currentFuel >= maxFuel)
            {
                return;
            }

            // Check if player has fuel needed in inventory
            var hasFuel = player.inventoryController.Inventory.HasItem(fuel, 1);
            if (hasFuel)
            {
                // remove fuel from player and add fuel level
                player.inventoryController.Inventory.TakeAndReturnRemaining(fuel, 1);
                currentFuel += 1;
            }

            UpdateFuelModels();
        }

        // Hide or show fuel models based on current level
        public void UpdateFuelModels()
        {
            for (int i=0; i<maxFuel; i++)
            {
                fuelModels[i].gameObject.SetActive(currentFuel > i);
            }
        }

        private void Awake()
        {
            UpdateFuelModels();
        }

        private void Update()
        {

        }
    }
}
