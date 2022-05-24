using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using UntoldTracks.Player;
using UntoldTracks.InventorySystem;


public interface IPlacableEntityController: IPlayerManangerComponent
{
    /// <summary>
    /// The placable object (if any) that is being placed by the player
    /// </summary>
    [CanBeNull]
    PlaceableEntity TargetPlacable { get; }
    
    /// <summary>
    /// Whether the player is attempting to place a placable entity
    /// </summary>
    bool IsPlacingSomething { get; }
    
    /// <summary>
    /// If the player is attempting to place a placable entity, then attempt to place that in the world
    /// </summary>
    /// <returns>Return true if the placable entity was succesfully placed</returns>
    bool TryPlace();
    
    /// <summary>
    /// Set the placable instance to become the active placable
    /// </summary>
    /// <param name="entity">An instance of the placable</param>
    void SetTargetPlacable(PlaceableEntity entity);
}

public class PlaceableEntityController : MonoBehaviour, IPlacableEntityController
{
    private PlayerManager _playerManager;
    private PlaceableEntity _targetPlaceable;
    [SerializeField]
    private float _rayLength = 0.2f;
    [SerializeField]
    private Material _canPlaceMaterial, _cantPlaceMaterial;

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
    
    public void Init(PlayerManager playerManager)
    {
        _playerManager = playerManager;
    }
    
    public bool IsPlaceable()
    {
        if (_targetPlaceable == null)
        {
            return false;
        }

        if (!_targetPlaceable.raycastOrigins.Any())
        {
            return false;
        }

        if (_targetPlaceable.IsTriggering)
        {
            return false;
        }

        foreach (var origin in _targetPlaceable.raycastOrigins)
        {
            Debug.DrawRay(origin.position, Vector3.down * _rayLength);

            var hit = Physics.Raycast(origin.position, Vector3.down, _rayLength);

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

        _targetPlaceable.transform.parent = transform.parent;
        _targetPlaceable.transform.localPosition = transform.localPosition;
        _targetPlaceable.transform.localEulerAngles = transform.localEulerAngles;

        transform.position = Vector3.zero;

        _targetPlaceable.transform.parent = _playerManager.InteractionController.LookingAtGameObject.transform;
        
        _targetPlaceable.ResetMaterials();
        
        _playerManager.PlayerActiveItem.activeItemObject = null;
        _playerManager.InventoryController.Inventory.Take(new ItemQuery(_targetPlaceable.source, 1, _playerManager.InventoryController.ActiveItem.Index));

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

    private void Update()
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

        _targetPlaceable.SetMaterial(IsPlaceable() ? _canPlaceMaterial : _cantPlaceMaterial);
    }
}
