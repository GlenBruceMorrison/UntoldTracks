using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.InventorySystem;
using UntoldTracks.Models;
using UntoldTracks.Managers;

public abstract class ConsumableEntity : EquipableEntityBase
{
    public int healthRestored;
    public int hungerRestored;
    public int thirtRestored;

    public virtual void HandleComsumption()
    {
        playerManager.InventoryController.Inventory.Take(new ItemQuery(playerManager.InventoryController.ActiveItemContainer.Item, 1));

        // set stats here!!
    }

    public override abstract void HandleEquip();
    public override abstract void HandleInteractionDown(InteractionData interaction);
    public override abstract void HandleInteractionUp(InteractionData interaction);
}
