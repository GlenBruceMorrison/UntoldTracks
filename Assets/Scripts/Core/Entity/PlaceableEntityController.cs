using UnityEngine;
using System.Linq;
using UntoldTracks.Player;
using UntoldTracks.InventorySystem;

public class PlaceableEntityController : MonoBehaviour
{
    public PlayerManager playerManager;
    public PlaceableEntity targetPlaceable;

    public float rayLength = 0.2f;
    
    public int placeableTurnSpeed = 5;

    public Material canPlaceMaterial, cantPlaceMaterial;
    public Material originalMaterial;

    public bool IsPlacingSomething
    {
        get
        {
            return targetPlaceable != null;
        }
    }
    
    public void Init(PlayerManager playerManager)
    {
        this.playerManager = playerManager;
    }
    
    public bool IsPlaceable()
    {
        if (targetPlaceable == null)
        {
            return false;
        }

        if (!targetPlaceable.raycastOrigins.Any())
        {
            return false;
        }

        if (targetPlaceable.IsTriggering)
        {
            return false;
        }

        foreach (var origin in targetPlaceable.raycastOrigins)
        {
            Debug.DrawRay(origin.position, Vector3.down * rayLength);

            var hit = Physics.Raycast(origin.position, Vector3.down, rayLength);

            if (!hit)
            {
                return false;
            }
        }

        return true;
    }

    public bool TryPlace()
    {
        if (!IsPlaceable())
        {
            return false;
        }

        targetPlaceable.transform.parent = transform.parent;
        targetPlaceable.transform.localPosition = transform.localPosition;
        targetPlaceable.transform.localEulerAngles = transform.localEulerAngles;

        transform.position = Vector3.zero;

        targetPlaceable.transform.parent = playerManager.interactionController.LookingAtGameObject.transform;
        
        targetPlaceable.ResetMaterials();
        
        playerManager.playerActiveItem.activeItemObject = null;
        playerManager.inventoryController.Inventory.Take(new ItemQuery(targetPlaceable.source, 1, playerManager.inventoryController.ActiveItem.Index));

        targetPlaceable.BeingPlaced = false;
        
        targetPlaceable = null;

        return true;
    }

    public void EquipPlaceable(PlaceableEntity entity)
    {
        ResetTransform(this.transform);

        targetPlaceable = entity;

        if (targetPlaceable == null)
        {
            Debug.LogError("A targetPlaceable is not set, so we are going to disable");
            this.gameObject.SetActive(false);
        }

        // todo: optimize
        targetPlaceable.GrabAllRenderers();
        targetPlaceable.SetMaterial(canPlaceMaterial);
        
        targetPlaceable.BeingPlaced = true;
        
        targetPlaceable.transform.parent = this.transform;
        ResetTransform(targetPlaceable.transform);
    }


    public void ResetTransform(Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    private void Update()
    {
        if (!IsPlacingSomething)
        {
            return;
        }

        if(Input.GetKeyUp("q"))
        {
            if (targetPlaceable.source.canRotate)
            {
                transform.localEulerAngles -= Vector3.up * 22.5f;
            }
        }
        else if (Input.GetKeyUp("r"))
        {
            if (targetPlaceable.source.canRotate)
            {
                transform.localEulerAngles += Vector3.up * 22.5f    ;
            }
        }

        var fromGround = targetPlaceable.transform.localScale.y/2;

        transform.position = new Vector3(
            playerManager.interactionController.LookingAtPosition.x,
            playerManager.interactionController.LookingAtPosition.y,
            playerManager.interactionController.LookingAtPosition.z);

        targetPlaceable.SetMaterial(IsPlaceable() ? canPlaceMaterial : cantPlaceMaterial);
    }
}
