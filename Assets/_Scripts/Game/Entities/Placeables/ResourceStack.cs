using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UntoldTracks;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Models;
using UntoldTracks.Managers;
using UnityEngine.Events;

public class ResourceStack : PlaceableEntity, IInteractable
{
    [SerializeField]
    private int _currentStored = 0;
    
    public Transform resourceContainer;
    public List<Transform> resource = new();
    public int Max => resource.Count;
    public ItemModel resourceItem;
    public string DisplayText => $"RMB [Remove {resourceItem.displayName}]\nLMB [Add {resourceItem.displayName}]";

    public Vector3 InteractionAnchor => transform.position;
    public List<InteractionDisplay> PossibleInputs => new()
    {
        new InteractionDisplay(InteractionInput.Action1, "Place"),
        new InteractionDisplay(InteractionInput.Secondary, "Take")
    };


    public event InteractionStateUpdate OnInteractionStateUpdate;

    private void Awake()
    {
        SetLogLevel();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    
    private void SetLogLevel()
    {
        var index = 0;
        foreach (Transform child in resourceContainer.transform)
        {
            resource.Add(child);

            if (_currentStored > index)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }

            index++;
        }
    }

    private bool AddLog()
    {
        if (_currentStored >= Max)
        {
            return false;
        }
        
        resource[_currentStored].gameObject.SetActive(true);
        
        _currentStored += 1;
        
        return true;
    }

    private bool RemoveLog()
    {
        if (_currentStored <= 0)
        {
            return false;
        }
        
        resource[_currentStored-1].gameObject.SetActive(false);
        
        _currentStored -= 1;
        
        return true;
    }
    
    public void HandleBecomeFocus(PlayerManager player) { }
    public void HandleLoseFocus(PlayerManager player) { }

    public void HandleInput(PlayerManager manager, InteractionData interaction)
    {
        switch (interaction.Input)
        {
            case InteractionInput.Action1:
                if (!manager.InventoryController.Inventory.CanTake(resourceItem, 1))
                {
                    return;
                }

                if (!AddLog())
                {
                    return;
                }

                manager.InventoryController.Inventory.Take(new ItemQuery(resourceItem, 1));
                break;

            case InteractionInput.Secondary:
                if (!manager.InventoryController.Inventory.CanGive(resourceItem, 1))
                {
                    return;
                }

                if (!RemoveLog())
                {
                    return;
                }

                manager.InventoryController.Inventory.Give(new ItemContainer(resourceItem, 1));
                break;
        }
    }
}
