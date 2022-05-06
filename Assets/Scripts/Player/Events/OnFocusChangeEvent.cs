using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFocusChangeEvent : PlayerEvent<OnFocusChangeEvent>
{
    public IInteractable newFocus;
    public IInteractable oldFocus;
}
