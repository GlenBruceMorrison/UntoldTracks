using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Managers;

public class InteractionData
{
    public InteractionInput Input { get; set; }
    public Vector3 InteractionPoint { get; set; }
    public Vector3 Origin { get; set; }
    public Vector3 Direction { get; set; }
    public float Distance { get; set; }
    public float Tangent { get; set; }
    
    public InteractionData(InteractionInput input, Vector3 interactionPoint, Vector3 origin)
    {
        Input = input;
        InteractionPoint = interactionPoint;
        Origin = origin;

        var diff = origin - interactionPoint;

        Direction = diff.normalized;
        Distance = diff.magnitude;

        //Debug.Log($"{origin} - {interactionPoint} = {Direction}");
    }
}

public abstract class EquipableEntityBase : Entity
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
    public abstract void HandleInteractionDown(InteractionData interaction);

    /// <summary>
    /// Called when the player releases an input button
    /// </summary>
    /// <param name="input">The input type was triggered</param>
    public abstract void HandleInteractionUp(InteractionData interaction);
    
    /// <summary>
    /// Called when this tool becomes the active item via the hotbar
    /// </summary>
    public abstract void HandleEquip();
}
