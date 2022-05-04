using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour, IPlayerControlController
{
    public PlayerManager playerManager;
    public IFirstPersonLook Look { get; private set; }
    public IFirstPersonMovement Movement { get; set; }

    private void Awake()
    {
        Look = GetComponentInChildren<FirstPersonLook>();
        Movement = GetComponentInChildren<FirstPersonMovement>();
    }
}
