using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.InventorySystem;
using UntoldTracks.Models;

public class Cup : EquipableEntityBase
{
    [SerializeField] Animator _animator;

    public ItemContainer currentCupState => toolData;
    public ItemModel waterCupData;
    public ItemModel emptyCupData;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public override void HandleEquip()
    {

    }

    public override void HandleInteractionDown(InteractionData interaction)
    {
        switch (interaction.Input)
        {
            // On primary drink
            case InteractionInput.Primary:
                if (currentCupState.Item != waterCupData)
                {
                    return;
                }

                _animator.Play("cup_drink");
                break;
            
            // On secondary empty
            case InteractionInput.Secondary:
                if (currentCupState.Item == waterCupData)
                {
                    return;
                }

                _animator.Play("cup_fill");
                break;
        }
    }

    public override void HandleInteractionUp(InteractionData interaction)
    {

    }

    public void HandleFillComplete()
    {
        if (currentCupState.Item == emptyCupData)
        {
            var activeIndex = playerManager.InventoryController.ActiveItemContainer.Index;

            playerManager.InventoryController.Inventory.Take(new ItemQuery(emptyCupData, 1, activeIndex));
            playerManager.InventoryController.Inventory.Give(new ItemContainer(waterCupData, 1));
        }
        else if (currentCupState.Item == waterCupData)
        {
            playerManager.InventoryController.ActiveItemContainer.SetDurability(999);
        }
    }

    public void HandleDrinkComplete()
    {
        if (playerManager.InventoryController.ActiveItemContainer.CurrentDurability == 1)
        {
            var activeIndex = playerManager.InventoryController.ActiveItemContainer.Index;

            playerManager.InventoryController.Inventory.Take(new ItemQuery(waterCupData, 1, activeIndex));
            playerManager.InventoryController.Inventory.Give(new ItemContainer(emptyCupData, 1));
        }
        else
        {
            playerManager.InventoryController.ActiveItemContainer.DecreaseDurability(1);
        }
    }
}
