using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;

public class Axe : UseableItem
{
    public Animator animator;
    public bool isSwinging = false;

    public override void HandleInputUp(InteractionInput input)
    {
        if (input == InteractionInput.Primary)
        {
            animator.Play("idle");
        }
    }

    public override void HandleInteractionDown(InteractionInput input)
    {
        if (input == InteractionInput.Primary)
        {
            animator.Play("strike");
        }
    }

    public override void HandleUnequip()
    {

    }

    public override void HandleEquip()
    {

    }
}
