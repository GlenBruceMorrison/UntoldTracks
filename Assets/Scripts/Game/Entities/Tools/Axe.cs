using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;

public class Axe : ToolEntity
{
    public Animator animator;

    public override void HandleInteractionDown(InteractionInput input)
    {
        if (input == InteractionInput.Primary)
        {
            animator.Play("axe_swing");
        }
    }

    public override void HandleInputUp(InteractionInput input)
    {

    }

    public override void HandleUnequip()
    {

    }

    public override void HandleEquip()
    {

    }
}
