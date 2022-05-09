using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UntoldTracks
{
    public class OnFocusChangeEvent : PlayerEvent<OnFocusChangeEvent>
    {
        public IInteractable newFocus;
        public IInteractable oldFocus;
    }
}
