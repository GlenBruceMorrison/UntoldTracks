using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;

public class PickAxe : EquipableEntity
{
    private Animator _animator;

    public bool CanSwingAgain { get; private set; }

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public override void HandleInteractionDown(InteractionInput input)
    {
        if (input == InteractionInput.Primary)
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("strike"))
            {
                Debug.Log("pre_strike");
                _animator.Play("pre_strike");
            }
            else
            {
                Debug.Log("strike");
                _animator.Play("strike");
                return;
            }
        }
    }

    public override void HandleInteractionUp(InteractionInput input)
    {

    }

    public override void HandleEquip()
    {

    }

    public void ReadyForAnotherSwing()
    {
        CanSwingAgain = true;
    }

    public void SwingOver()
    {
        CanSwingAgain = false;
    }
}
