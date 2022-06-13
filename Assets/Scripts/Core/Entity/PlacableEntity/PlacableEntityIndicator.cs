using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlacableEntityIndicator : MonoBehaviour
{
    private Collider _collider;

    public UnityEvent TriggerStay, TriggerExit;

    private void Awake()
    {
        _collider = GetComponent<Collider>();

        if (!_collider.isTrigger)
        {
            Debug.LogWarning("This collider is supposed to be a trigger, so setting it to a trigger");
            _collider.isTrigger = true;
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        TriggerStay?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exit");
        TriggerExit?.Invoke();
    }
}
