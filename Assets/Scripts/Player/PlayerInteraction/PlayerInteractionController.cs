using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [HideInInspector]
    public PlayerManager playerManager;

    public float interactionDistance = 300f;

    private void Update()
    {
        if (playerManager.FirstPersonController.Look.IsPointerLocked())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.DrawRay(transform.position, transform.forward * interactionDistance, Color.red);

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance))
            {
                if (hit.collider.TryGetComponent(out ItemContainerWorldObject itemContainerWorldObject))
                {
                    playerManager.inventoryController.HandleItemPickupFromWorld(itemContainerWorldObject);
                }
                else if (hit.collider.TryGetComponent(out GenericInventoryController genericInventoryController))
                {
                    playerManager.inventoryController.AccessInventory(genericInventoryController);
                }
            }
        }
    }
}
