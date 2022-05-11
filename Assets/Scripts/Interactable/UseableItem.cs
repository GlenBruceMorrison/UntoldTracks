using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;

public class UseableItem : MonoBehaviour
{
    public virtual void HandleInputUp(InteractionInput input)
    {

    }

    public virtual void HandleInteractionDown(InteractionInput input)
    {

    }

    public virtual void TriggerInteraction()
    {
        var playerManager = GetComponentInParent<UntoldTracks.Player.PlayerManager>();
        playerManager.interactionController.TriggerCurrentFocus();
    }

    public virtual void HandleUnequip()
    {

    }

    public virtual void HandleEquip()
    {

    }
}
