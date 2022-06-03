using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.Player;
using System.Linq;
using System.IO;
using UntoldTracks.Models;
using UntoldTracks.Managers;

public class PlanterSpot : Entity, IInteractable
{
    public Plant currentPlant;
    public List<PlantModel> possiblePlants = new List<PlantModel>();


    public bool HasPlant => currentPlant != null;

    public Sprite DisplaySprite => null;

    public List<InteractionDisplay> PossibleInputs => new List<InteractionDisplay>()
    {
        new InteractionDisplay(InteractionInput.Action1, "")
    };

    public void HandleBecomeFocus(PlayerManager player) { }

    public void HandleInput(PlayerManager manager, InteractionInput input)
    {
        if (input == InteractionInput.Action1)
        {
            var activeItem = manager.InventoryController.ActiveItemContainer.Item;
            if (CanPlantHere(activeItem))
            {
                var plant = Instantiate(GetPlantFromItem(activeItem), this.transform);
                plant.transform.localPosition = Vector3.zero;
            }
        }
    }

    public bool CanPlantHere(ItemModel item)
    {
        return possiblePlants.Where(x => x.seed == item).Any();
    }

    public Plant GetPlantFromItem(ItemModel item)
    {
        return possiblePlants.FirstOrDefault(x => x.seed == item)?.plant;
    }

    public void HandleLoseFocus(PlayerManager player)
    {

    }

    public void Plant(Plant plant)
    {
        if (HasPlant)
        {
            return;
        }

        currentPlant = plant;
    }
}
