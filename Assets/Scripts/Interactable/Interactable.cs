using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    public UnityAction<PlayerManager> onInteract, onLoseFocus, onGainFocus;

    public void HandleBecomeFocus(PlayerManager player)
    {
        onGainFocus?.Invoke(player);
    }

    public void HandleInteraction(PlayerManager player)
    {
        onInteract?.Invoke(player);
    }

    public void HandleLoseFocus(PlayerManager player)
    {
        onLoseFocus?.Invoke(player);
    }
}
