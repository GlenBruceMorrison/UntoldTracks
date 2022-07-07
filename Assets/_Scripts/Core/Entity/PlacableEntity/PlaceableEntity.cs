using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UntoldTracks.Models;
using SimpleJSON;
using UntoldTracks.Managers;

using UnityEngine;

[System.Serializable]
public class RayCaster
{
    public Transform origin;
    public Direction direction = Direction.Down;
}

public enum Direction
{
    Up,
    Left,
    Right,
    Down,
    Forward,
    Backward
}

public class PlaceableEntity : Entity, ITokenizable
{
    [Header("The ItemModel that this is created from")]
    public ItemModel source;

    [Header("Raycasts that have to be hit for this to be placeable")]
    public List<Transform> raycastOrigins = new();

    [Header("Colliders that are turned off when in placeable state")]
    public List<Collider> ignoreWhenPlacing = new();

    [Header("The model prefab")]
    [SerializeField]
    private Transform _worldTransform;

    private Renderer[] _allRenderers;
    private Material[] _allMaterials;
    private bool _beingPlaced;
    private bool _isTriggering = false;

    private ColliderEvents _colliderEvents;


    [HideInInspector]
    public Material[] AllMaterials
    {
        get
        {
            return _allMaterials;
        }
    }

    public bool IsTriggering
    {
        get
        {
            return _isTriggering;
        }
    }

    public bool BeingPlaced
    {
        get
        {
            return _beingPlaced;
        }
        set
        {
            SetBeingPlaced(value);
        }
    }

    private void Awake()
    {
        GrabAllRenderers();
    }

    public override void Start()
    {
        base.Start();
    }

    public void AfterBuild()
    {
        transform.SetParent(App.TrainManager.Train.GetClosestCarriage(transform).transform, true);
    }

    protected virtual void OnEnable()
    {
        if (_worldTransform != null)
        {
            _colliderEvents = _worldTransform.GetComponent<ColliderEvents>();

            if (_colliderEvents == null)
            {
                _colliderEvents = _worldTransform.gameObject.AddComponent<ColliderEvents>();
            }

            if (_colliderEvents != null)
            {
                _colliderEvents.TriggerStay += (HandleTriggerStay);
                _colliderEvents.TriggerExit += (HandleTriggerExit);
            }
            else
            {
                throw new Exception("Could not get a ColliderEvents for this placeable, events not subscribed to");
            }
        }
        else
        {
            throw new Exception("This placeable does not have a world object assigned in this script");
        }
    }

    protected virtual void OnDisable()
    {
        _colliderEvents.TriggerStay -= (HandleTriggerStay);
        _colliderEvents.TriggerExit -= (HandleTriggerExit);
    }

    public void HandleTriggerStay()
    {
        _isTriggering = true;
    }

    public void HandleTriggerExit()
    {
        _isTriggering = false;
    }

    public void SetMaterial(Material material)
    {
        foreach (var t in _allRenderers)
        {
            t.material = material;
        }
    }
 
    public void ResetMaterials()
    {
        for (var i = 0; i < _allRenderers.Length; i++)
        {
            _allRenderers[i].material = AllMaterials[i];
        }
    }

    public void GrabAllRenderers()
    {
        _allRenderers = GetComponentsInChildren<Renderer>();
        _allMaterials = new Material[_allRenderers.Length];

        for (var i = 0; i < _allRenderers.Length; i++)
        {
            _allMaterials[i] = _allRenderers[i].material;
        }
    }

    private void SetBeingPlaced(bool state)
    {
        if (_beingPlaced == state)
        {
            return;
        }
            
        _beingPlaced = state;
            
        if (_beingPlaced)
        {
            _worldTransform.gameObject.SetLayerRecursively("Ignore Raycast");
            ignoreWhenPlacing.ForEach(x => x.enabled = false);
            gameObject.ChildCollidersToTriggers(true);
        }
        else
        {
            Utility.SetLayerRecursively(_worldTransform.gameObject, LayerMask.NameToLayer("Default"));
            ignoreWhenPlacing.ForEach(x => x.enabled = true);
            gameObject.ChildCollidersToTriggers(false);
        }
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        foreach (var ray in raycastOrigins)
        {
            Gizmos.DrawLine(
                ray.transform.position,
                ray.transform.position + (-ray.up*0.2f)
            );
        }

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(7, 0.01f, 7));
    }

    #region Token
    public override void Load(JSONNode node)
    {
        base.Load(node);
    }

    public override JSONObject Save()
    {
        var node = base.Save();
        node.Add("itemGUID", source.Guid);
        return node;
    }
    #endregion
}
