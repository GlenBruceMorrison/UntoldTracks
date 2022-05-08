using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour, IFirstPersonController
{
    [HideInInspector]
    public PlayerManager playerManager;
    public IFirstPersonController Look { get; private set; }
    public IFirstPersonMovement Movement { get; set; }

    public bool IsPointerLocked()
    {
        return false;
    }

    public void LockPointer()
    {

    }

    public void UnlockPointer()
    {

    }

    private void Awake()
    {
        Look = GetComponentInChildren<FirstPersonLook>();
        Movement = GetComponentInChildren<FirstPersonMovement>();
    }
}
