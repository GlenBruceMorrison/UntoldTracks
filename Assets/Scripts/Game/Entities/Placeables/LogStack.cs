using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;

public class LogStack : PlaceableEntity, IInteractable
{
    private int _currentStored = 0;
    
    public Transform logContainer;
    public List<Transform> logs = new List<Transform>();
    public int Max => logs.Count;
    public Item logItem;
    public string DisplayText => "RMB [Remove Log]\nLMB [Add Log]";
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
        foreach (Transform child in logContainer.transform)
        {
            logs.Add(child);

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
        
        logs[_currentStored].gameObject.SetActive(true);
        
        _currentStored += 1;
        
        return true;
    }

    private bool RemoveLog()
    {
        if (_currentStored <= 0)
        {
            return false;
        }
        
        logs[_currentStored-1].gameObject.SetActive(false);
        
        _currentStored -= 1;
        
        return true;
    }
    
    public void HandlePrimaryInput(PlayerManager player, ItemContainer usingContainer)
    {
        if (!player.InventoryController.Inventory.CanTake(logItem, 1))
        {
            return;
        }

        if (!AddLog())
        {
            return;
        }

        player.InventoryController.Inventory.Take(new ItemQuery(logItem, 1));
    }
    
    public void HandleSecondaryInput(PlayerManager player, ItemContainer usingContainer)
    {
        if (!player.InventoryController.Inventory.CanGive(logItem, 1))
        {
            return;
        }

        if (!RemoveLog())
        {
            return;
        }
        
        player.InventoryController.Inventory.Give(new ItemContainer(logItem, 1));
    }
    
    public void HandleBecomeFocus(PlayerManager player) { }
    public void HandleLoseFocus(PlayerManager player) { }
}
