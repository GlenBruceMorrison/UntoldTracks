using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using UntoldTracks.Player;
using UntoldTracks.InventorySystem;


public interface IPlacableEntityController
{
    /// <summary>
    /// The placable object (if any) that is being placed by the player
    /// </summary>
    PlaceableEntity TargetPlacable { get; }
    
    /// <summary>
    /// Whether the player is attempting to place a placable entity
    /// </summary>
    bool IsPlacingSomething { get; }
    
    /// <summary>
    /// If the player is attempting to place a placable entity, then attempt to place that in the world
    /// </summary>
    /// <returns>Return true if the placable entity was succesfully placedz</returns>
    bool TryPlace();
    
    /// <summary>
    /// Set the placable instance to become the active placable
    /// </summary>
    /// <param name="entity">An instance of the placable</param>
    void SetTargetPlacable(PlaceableEntity entity);
}

public class PlaceableEntityController : PlayerComponent, IPlacableEntityController
{
    private PlaceableEntity _targetPlaceable;
    [SerializeField] private float _rayLength = 0.2f;
    [SerializeField] private Material _canPlaceMaterial, _cantPlaceMaterial;
    
    public PlaceableEntity TargetPlacable
    {
        get
        {
            return _targetPlaceable;
        }
    }

    public bool IsPlacingSomething
    {
        get
        {
            return _targetPlaceable != null;
        }
    }
    
    private bool IsPlaceable()
    {
        if (_targetPlaceable == null)
        {
            return false;
        }

        if (!_targetPlaceable.raycastOrigins.Any())
        {
            return false;
        }

        // if this is colliding with something, then return
        if (_targetPlaceable.IsTriggering)
        {
            return false;
        }

        // loop through each ray on the active placable
        foreach (var origin in _targetPlaceable.raycastOrigins)
        {
            Debug.DrawRay(origin.position, Vector3.down * _rayLength);

            // chek if that ray has succesfully hit something
            var hit = Physics.Raycast(origin.position, Vector3.down, _rayLength);

            // if not then return this as not placable
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

        // set transform values
        _targetPlaceable.transform.parent = transform.parent;
        _targetPlaceable.transform.localPosition = transform.localPosition;
        _targetPlaceable.transform.localEulerAngles = transform.localEulerAngles;
        Destroy(_targetPlaceable.GetComponent<Rigidbody>());
        transform.position = Vector3.zero;

        // set parent to object placed on
        _targetPlaceable.transform.parent = _playerManager.InteractionController.LookingAtGameObject.transform;
        
        // remove can place and cant place material indicators
        _targetPlaceable.ResetMaterials();
        
        // remove this as the players active item
        //todo: Can we just reove the item from inventory and this is handled by that event?
        _playerManager.PlayerActiveItemController.activeItemObject = null;
        
        // remove this placable from the players inventory
        _playerManager.InventoryController.Inventory.Take(new ItemQuery(_targetPlaceable.source, 1, _playerManager.InventoryController.ActiveItem.Index));

        // reset placable values
        _targetPlaceable.BeingPlaced = false;
        _targetPlaceable = null;

        return true;
    }

    public void SetTargetPlacable(PlaceableEntity entity)
    {
        ResetTransform(this.transform);

        _targetPlaceable = entity;

        if (_targetPlaceable == null)
        {
            Debug.LogError("A targetPlaceable is not set, so we are going to disable");
            this.gameObject.SetActive(false);
        }

        // todo: optimize
        _targetPlaceable.GrabAllRenderers();
        _targetPlaceable.SetMaterial(_canPlaceMaterial);
        
        _targetPlaceable.BeingPlaced = true;
        
        _targetPlaceable.transform.parent = this.transform;
        ResetTransform(_targetPlaceable.transform);
    }

    private void ResetTransform(Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    #region Player Component
    protected override void Run(float deltaTime)
    {
        if (!IsPlacingSomething)
        {
            return;
        }

        if(Input.GetKeyUp("q"))
        {
            if (_targetPlaceable.source.canRotate)
            {
                transform.localEulerAngles -= Vector3.up * 22.5f;
            }
        }
        else if (Input.GetKeyUp("r"))
        {
            if (_targetPlaceable.source.canRotate)
            {
                transform.localEulerAngles += Vector3.up * 22.5f    ;
            }
        }

        transform.position = _playerManager.InteractionController.LookingAtVector;

        // todo: optimise, we don't need to change these renderers ever frame only when the state changes
        _targetPlaceable.SetMaterial(IsPlaceable() ? _canPlaceMaterial : _cantPlaceMaterial);
    }
    #endregion
}
