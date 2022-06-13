using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Managers;

public abstract class EquipableEntity : Entity
{
    internal PlayerManager playerManager;
    [HideInInspector] public ItemContainer toolData;

    public void EquipedByPlayer(PlayerManager playerManager)
    {
        this.playerManager = playerManager;
        HandleEquip();
    }

    /// <summary>
    /// Must be called to trigger the interaction the player is currently looking at
    /// can be overriden to change how interactions should function for this specific
    /// tool
    /// </summary>
    public virtual void TriggerInteraction()
    {
        playerManager.InteractionController.TriggerCurrentFocus();
        //container.DecreaseDurability(1);
    }

    /// <summary>
    /// Called when a player presses down on an input button
    /// </summary>
    /// <param name="input">The input type was triggered</param>
    public abstract void HandleInteractionDown(InteractionInput input);

    /// <summary>
    /// Called when the player releases an input button
    /// </summary>
    /// <param name="input">The input type was triggered</param>
    public abstract void HandleInteractionUp(InteractionInput input);
    
    /// <summary>
    /// Called when this tool becomes the active item via the hotbar
    /// </summary>
    public abstract void HandleEquip();
}
