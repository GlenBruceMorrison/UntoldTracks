using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;

public class LogStack : PlaceableEntity, IInteractable
{
    public Transform logContainer;
    public List<Transform> logs = new List<Transform>();

    public int currentStored = 0;
    public int Max => logs.Count;
    public Item logItem;
    public string DisplayText => "RMB [Remove Log]\nLMB [Add Log]";
    public Sprite DisplaySprite => source.sprite;
    
    private void Awake()
    {
        var index = 0;
        foreach (Transform child in logContainer.transform)
        {
            logs.Add(child);

            if (currentStored > index)
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

    public bool AddLog()
    {
        if (currentStored >= Max)
        {
            return false;
        }
        
        logs[currentStored].gameObject.SetActive(true);
        
        currentStored += 1;
        
        return true;
    }

    public bool RemoveLog()
    {
        if (currentStored <= 0)
        {
            return false;
        }
        
        logs[currentStored-1].gameObject.SetActive(false);
        
        currentStored -= 1;
        
        return true;
    }
    
    public void HandlePrimaryInput(PlayerManager player, ItemContainer usingContainer)
    {
        if (!player.inventoryController.Inventory.CanTake(logItem, 1))
        {
            return;
        }

        if (!AddLog())
        {
            return;
        }

        player.inventoryController.Inventory.Take(new ItemQuery(logItem, 1));
    }
    
    public void HandleSecondaryInput(PlayerManager player, ItemContainer usingContainer)
    {
        if (!player.inventoryController.Inventory.CanGive(logItem, 1))
        {
            return;
        }

        if (!RemoveLog())
        {
            return;
        }
        
        player.inventoryController.Inventory.Give(new ItemContainer(logItem, 1));
    }
    
    public void HandleBecomeFocus(PlayerManager player) { }
    public void HandleLoseFocus(PlayerManager player) { }
}
