﻿using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.Serialization;
using UntoldTracks.Player;
using UntoldTracks.InventorySystem;
using UntoldTracks.Managers;

public class PlaceableEntityController : MonoBehaviour
{
    public bool limitPlacingToCarriage = true;

    public PlayerManager playerManager;

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
            Debug.LogError("This placeable does not have any raycast origins attatched, it will never be placeable");
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
            // chek if that ray has succesfully hit something
            var hit = Physics.Raycast(origin.position, -origin.up, _rayLength);

            // if not then return this as not placable
            if (!hit)
            {
                Debug.DrawRay(origin.position, -origin.up * _rayLength, Color.red);
                return false;
            }

            Debug.DrawRay(origin.position, -origin.up * _rayLength, Color.green);
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
        _targetPlaceable.transform.parent = playerManager.InteractionController.LookingAtGameObject.transform;

        // remove can place and cant place material indicators
        Debug.Log("Rseting MAts");
        _targetPlaceable.ResetMaterials();

        // remove this as the players active item
        //todo: Can we just reove the item from inventory and this is handled by that event?
        playerManager.PlayerActiveItemController.activeItemObject = null;

        // remove this placable from the players inventory
        playerManager.InventoryController.Inventory.Take(new ItemQuery(_targetPlaceable.source, 1, playerManager.InventoryController.ActiveItemContainer.Index));

        GameManager.Instance.TrainManager.PlaceableEntityManager.entities.Add(_targetPlaceable);
        GameManager.Instance.TrainManager.PlaceableEntityManager.tokens.Add(_targetPlaceable);

        // reset placable values
        _targetPlaceable.BeingPlaced = false;
        _targetPlaceable = null;

        return true;
    }

    public void SetTargetPlacable(PlaceableEntity entity)
    {
        ResetTransform(transform);

        _targetPlaceable = entity;

        if (_targetPlaceable == null)
        {
            Debug.LogError("A targetPlaceable is not set, so we are going to disable");
            gameObject.SetActive(false);
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

    #region Life Cycle
    private void LateUpdate()
    {
        if (!IsPlacingSomething)
        {
            return;
        }

        if (playerManager.InteractionController.LookingAtGameObject == null)
        {
            _targetPlaceable.gameObject.SetActive(false);
            return;
        }

        if (limitPlacingToCarriage)
        {
            if (playerManager.InteractionController.LookingAtGameObject.GetComponentInParent<Carriage>() == null)
            {
                _targetPlaceable.gameObject.SetActive(false);
                return;
            }

            transform.parent = playerManager.InteractionController.LookingAtGameObject.GetComponentInParent<Carriage>().transform;
        }

        _targetPlaceable.gameObject.SetActive(true);
        transform.position = playerManager.InteractionController.LookingAtVector;

        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (_targetPlaceable.source.canRotate)
            {
                transform.localEulerAngles -= Vector3.up * 22.5f;
            }
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            if (_targetPlaceable.source.canRotate)
            {
                transform.localEulerAngles += Vector3.up * 22.5f;
            }
        }

        // todo: optimise, we don't need to change these renderers ever frame only when the state changes
        _targetPlaceable.SetMaterial(IsPlaceable() ? _canPlaceMaterial : _cantPlaceMaterial);
    }
    #endregion
}
