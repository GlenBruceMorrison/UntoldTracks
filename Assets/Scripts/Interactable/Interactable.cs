using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    public UnityEvent<PlayerManager> onInteract;

    public void HandleInteraction(PlayerManager player)
    {
        onInteract.Invoke(player);
    }
}
