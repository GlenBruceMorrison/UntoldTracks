using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;

public class Axe : EquipableEntityBase
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public override void HandleInteractionDown(InteractionData interaction)
    {
        if (interaction.Input == InteractionInput.Primary)
        {
            _animator.Play("axe_swing");
        }
    }

    public override void HandleInteractionUp(InteractionData interaction)
    {

    }

    public override void HandleEquip()
    {

    }
}
