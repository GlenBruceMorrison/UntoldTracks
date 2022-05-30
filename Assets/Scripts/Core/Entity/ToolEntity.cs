using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.InventorySystem;

public class ToolEntity : Entity
{
    public ItemContainer container;

    public virtual void HandleInputUp(InteractionInput input)
    {

    }

    public virtual void HandleInteractionDown(InteractionInput input)
    {

    }

    public virtual void TriggerInteraction()
    {
        Debug.Log("hit");
        var playerManager = GetComponentInParent<UntoldTracks.Player.PlayerManager>();
        playerManager.InteractionController.TriggerCurrentFocus();
        //container.DecreaseDurability(1);
    }

    public virtual void HandleUnequip()
    {

    }

    public virtual void HandleEquip()
    {

    }
}
