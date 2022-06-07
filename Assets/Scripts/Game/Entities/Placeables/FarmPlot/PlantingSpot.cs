using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UntoldTracks.InventorySystem;
using UntoldTracks.Managers;
using UntoldTracks.Models;

public partial class PlantingSpot : Entity
{
    public List<PlantRecipe> possiblePlants = new();
    public PlantRecipe current;

    public float growthMultiplier = 1.0f;

    public bool IsPickable
    {
        get
        {
            if (current == null)
            {
                return false;
            }

            return current.IsPickable;
        }
    }

    public PlantRecipe GetPlantFromItem(ItemModel item)
    {
        foreach (var plant in possiblePlants)
        {
            if (plant.input == item)
            {
                return plant;
            }
        }

        return null;
    }

    public bool CanPlant(ItemModel item)
    {
        if (current != null)
        {
            return false;
        }

        if (GetPlantFromItem(item) != null)
        {
            return true;
        }

        return false;
    }

    public void Pick(PlayerManager player)
    {
        if (!IsPickable)
        {
            return;
        }

        foreach (var output in current.outputs)
        {
            player.InventoryController.Inventory.Give(new ItemContainer(output, 1));
        }

        current.Reset();
        current.transform.gameObject.SetActive(false);
        current = null;
    }

    public void Plant(PlayerManager player)
    {
        if (current != null)
        {
            Pick(player);
            return;
        }

        var item = player.InventoryController.ActiveItemContainer.Item;

        var plant = GetPlantFromItem(item);

        if (plant == null)
        {
            return;
        }

        player.InventoryController.Inventory.Take(new ItemQuery(item, 1));

        current = plant;
        current.transform.gameObject.SetActive(true);
        current.Reset();
    }

    private void Update()
    {
        if (current == null)
        {
            return;
        }

        current.Grow(Time.deltaTime * growthMultiplier);
    }
}
