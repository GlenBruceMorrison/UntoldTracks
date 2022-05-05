using System;
public interface IInteractable
{
    public void HandleInteraction(PlayerManager player);
    public void HandleBecomeFocus(PlayerManager player);
    public void HandleLoseFocus(PlayerManager player);
}
