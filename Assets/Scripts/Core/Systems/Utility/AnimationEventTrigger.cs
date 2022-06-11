using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventTrigger : MonoBehaviour
{
    public UnityEvent Trigger1, Trigger2, Trigger3;

    public void TriggerEvent1()
    {
        Trigger1?.Invoke();
    }

    public void TriggerEvent2()
    {
        Trigger2?.Invoke();
    }

    public void TriggerEvent3()
    {
        Trigger3?.Invoke();
    }
}
