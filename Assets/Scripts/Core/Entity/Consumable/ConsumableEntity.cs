using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.InventorySystem;

public abstract class ConsumableEntity : EquipableEntity
{
    public int healthRestored;
    public int hungerRestored;
    public int thirtRestored;

    public virtual void HandleComsumption()
    {
        playerManager.InventoryController.Inventory.Take(
            new ItemQuery(playerManager.InventoryController.ActiveItemContainer.Item, 1));

        // set stats here!!
    }

    public override abstract void HandleEquip();
    public override abstract void HandleInteractionDown(InteractionInput input);
    public override abstract void HandleInteractionUp(InteractionInput input);
}
