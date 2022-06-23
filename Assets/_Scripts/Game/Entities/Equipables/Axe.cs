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

    public override void HandleInteractionDown(InteractionInput input)
    {
        if (input == InteractionInput.Primary)
        {
            _animator.Play("axe_swing");
        }
    }

    public override void HandleInteractionUp(InteractionInput input)
    {

    }

    public override void HandleEquip()
    {

    }
}
