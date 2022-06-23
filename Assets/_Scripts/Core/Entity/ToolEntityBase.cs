using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Managers;
using System;

public abstract class ToolEntityBase : EquipableEntityBase
{
    [SerializeField] internal Animator _animator;

    public virtual void OnEnable()
    {
        InputService.Instance.OnPrimaryUp += HandlePrimaryUp;
    }

    private void HandlePrimaryUp()
    {
        _animator.SetInteger("state", 0);
    }

    private void OnDisable()
    {
        InputService.Instance.OnPrimaryUp -= HandlePrimaryUp;
    }

    public override void HandleInteractionDown(InteractionInput input)
    {
        if (input == InteractionInput.Primary)
        {
            _animator.SetInteger("state", 1);
        }
    }

    public override void HandleInteractionUp(InteractionInput input)
    {
        _animator.SetInteger("state", 0);
    }

    public override void HandleEquip()
    {

    }
}
