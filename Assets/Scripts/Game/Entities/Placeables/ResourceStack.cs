using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UntoldTracks;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;

public class ResourceStack : PlaceableEntity, IInteractable
{
    [SerializeField]
    private int _currentStored = 0;
    
    public Transform resourceContainer;
    public List<Transform> resource = new List<Transform>();
    public int Max => resource.Count;
    public Item resourceItem;
    public string DisplayText => $"RMB [Remove {resourceItem.name}]\nLMB [Add {resourceItem.name}]";
    public Sprite DisplaySprite => source.sprite;

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
    
    public void HandlePrimaryInput(PlayerManager player, ItemContainer usingContainer)
    {
        if (!player.InventoryController.Inventory.CanTake(resourceItem, 1))
        {
            return;
        }

        if (!AddLog())
        {
            return;
        }

        player.InventoryController.Inventory.Take(new ItemQuery(resourceItem, 1));
    }
    
    public void HandleSecondaryInput(PlayerManager player, ItemContainer usingContainer)
    {
        if (!player.InventoryController.Inventory.CanGive(resourceItem, 1))
        {
            return;
        }

        if (!RemoveLog())
        {
            return;
        }
        
        player.InventoryController.Inventory.Give(new ItemContainer(resourceItem, 1));
    }
    
    public void HandleBecomeFocus(PlayerManager player) { }
    public void HandleLoseFocus(PlayerManager player) { }
}
