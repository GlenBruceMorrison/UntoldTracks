using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerInventoryController inventoryController;
    public PlayerControlController controlController;
    public PlayerInteractionController interactionController;


    private void Awake()
    {
        inventoryController = GetComponentInChildren<PlayerInventoryController>();
        if (inventoryController == null)
        {
            throw new System.Exception("This player mananger must have an inventory controller attatched");
        }
        inventoryController.playerManager = this;

        controlController = GetComponentInChildren<PlayerControlController>();
        if (controlController == null)
        {
            throw new System.Exception("This player mananger must have an control controller attatched");
        }
        controlController.playerManager = this;

        interactionController = GetComponentInChildren<PlayerInteractionController>();
        if (interactionController == null)
        {
            throw new System.Exception("This player mananger must have an interaction controller attatched");
        }
        interactionController.playerManager = this;
    }
}
