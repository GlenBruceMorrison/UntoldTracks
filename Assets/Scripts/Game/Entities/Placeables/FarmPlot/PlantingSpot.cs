using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks;
using UntoldTracks.InventorySystem;
using UntoldTracks.Managers;
using UntoldTracks.Models;

public partial class PlantingSpot : Entity, IInteractable
{
    public List<PlantRecipe> possiblePlants = new();
    public PlantRecipe current;

    public float growthMultiplier = 1.0f;

    public event InteractionStateUpdate OnInteractionStateUpdate;

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

    private List<InteractionDisplay> _possibleInputs;
    public List<InteractionDisplay> PossibleInputs => _possibleInputs;

    public Vector3 InteractionAnchor => this.transform.position;

    public UnityAction OnCropGrabbed, OnCropPlanted;

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

        current.OnPlantGrown -= HandleStateUpdate;

        current.Reset();
        current.transform.gameObject.SetActive(false);
        current = null;

        OnCropGrabbed?.Invoke();
        HandleStateUpdate();
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

        plant.OnPlantGrown += HandleStateUpdate;

        OnCropPlanted?.Invoke();
        HandleStateUpdate();
    }

    private void Update()
    {
        if (current == null)
        {
            return;
        }

        current.Grow(Time.deltaTime * growthMultiplier);
    }

    public void HandleStateUpdate()
    {
        var activeItem = GameManager.Instance.LocalPlayer.InventoryController.ActiveItemContainer.Item;

        if (current == null)
        {
            if (activeItem == null)
            {
                _possibleInputs = new();
            }
            else
            {
                var possible = possiblePlants.FirstOrDefault(x => x.input == activeItem);

                if (possible != null)
                {
                    _possibleInputs = new()
                    {
                        new InteractionDisplay(InteractionInput.Action1, $"Plant {activeItem.displayName}")
                    };
                }
                else
                {
                    _possibleInputs = new();
                }
            }
        }
        else if (IsPickable)
        {
            _possibleInputs = new()
            {
                new InteractionDisplay(InteractionInput.Action1, $"Pick {current.outputs.First().displayName}")
            };
        }
        else
        {
            _possibleInputs = new();
        }

        OnInteractionStateUpdate?.Invoke();
    }

    public void HandleInput(PlayerManager manager, InteractionInput input)
    {
        if (input != InteractionInput.Action1) return;
        Plant(manager);
    }

    public void HandleBecomeFocus(PlayerManager player)
    {
        HandleStateUpdate();
    }

    public void HandleLoseFocus(PlayerManager player)
    {

    }
}
