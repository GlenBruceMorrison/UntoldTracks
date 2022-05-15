using UnityEngine;
using System.Linq;
using UntoldTracks.Player;

public class PlaceableEntityController : MonoBehaviour
{
    public PlayerInteractionController interactionController;
    public PlaceableEntity targetPlaceable;

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

        foreach (var origin in targetPlaceable.raycastOrigins)
        {
            Debug.DrawRay(origin.position, Vector3.down * 1f);

            var hit = Physics.Raycast(origin.position, Vector3.down, 1);

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

        targetPlaceable.transform.parent = this.transform.parent;
        targetPlaceable.transform.localPosition = this.transform.localPosition;
        targetPlaceable.transform.localEulerAngles = this.transform.localEulerAngles;

        transform.position = Vector3.zero;

        GameObject.FindObjectOfType<PlayerActiveItem>().activeItemObject = null;
        GameObject.FindObjectOfType<PlayerManager>().inventoryController.Inventory.TakeAndReturnRemaining(targetPlaceable.source, 1);

        targetPlaceable = null;

        return true;
    }

    public void EquipPlaceable(PlaceableEntity entity)
    {
        targetPlaceable = entity;

        if (targetPlaceable == null)
        {
            Debug.LogError("A targetPlaceable is not set, so we are going to disable");
            this.gameObject.SetActive(false);
        }

        targetPlaceable.transform.parent = this.transform;
        targetPlaceable.transform.localPosition = Vector3.zero;
        targetPlaceable.transform.localEulerAngles = Vector3.zero;

        targetPlaceable.OnEntityMove += HandleEntityMove;
    }

    private void OnDisable()
    {
        
    }

    private void HandleEntityMove(Vector3 old, Vector3 current)
    {
        //var canPlace = IsPlaceable();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TryPlace())
            {
                //this.gameObject.SetActive(false);
            }
        }

        if (targetPlaceable == null)
        {
            return;
        }

        var fromGround = targetPlaceable.transform.localScale.y/2;

        transform.position = new Vector3(
            interactionController.LookingAt.x,
            interactionController.LookingAt.y + fromGround,
            interactionController.LookingAt.z);

        Debug.Log(IsPlaceable());
    }
}
