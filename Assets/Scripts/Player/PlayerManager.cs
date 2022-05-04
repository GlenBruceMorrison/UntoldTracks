using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerInventoryController inventoryController;
    public FirstPersonController FirstPersonController;
    public PlayerInteractionController interactionController;


    private void Awake()
    {
        inventoryController = GetComponentInChildren<PlayerInventoryController>();
        if (inventoryController == null)
        {
            throw new System.Exception("This player mananger must have an inventory controller attatched");
        }
        inventoryController.playerManager = this;

        FirstPersonController = GetComponentInChildren<FirstPersonController>();
        if (FirstPersonController == null)
        {
            throw new System.Exception("This player mananger must have an control controller attatched");
        }
        FirstPersonController.playerManager = this;

        interactionController = GetComponentInChildren<PlayerInteractionController>();
        if (interactionController == null)
        {
            throw new System.Exception("This player mananger must have an interaction controller attatched");
        }
        interactionController.playerManager = this;
    }
}
