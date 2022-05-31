using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;

public class BasicFood : ConsumableEntity
{
    public string consumeAnimationName, idleAnimationName;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public override void HandleEquip()
    {

    }

    public override void HandleInteractionDown(InteractionInput input)
    {
        if (input == InteractionInput.Primary)
            _animator.Play(consumeAnimationName);
    }

    public override void HandleInteractionUp(InteractionInput input)
    {
        Debug.Log("up");
        if (input == InteractionInput.Primary)
            _animator.Play(idleAnimationName);
    }
}
