using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderEvents : MonoBehaviour
{
    private Collider _collider;

    public UnityAction TriggerStay, TriggerExit;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
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
