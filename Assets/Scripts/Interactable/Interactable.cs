using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    private string _displayText;
    private Sprite _displaySprite;

    public string DisplayText => _displayText;
    public Sprite DisplaySprite => _displaySprite;

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
