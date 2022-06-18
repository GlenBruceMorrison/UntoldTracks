using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UntoldTracks.Models;
using SimpleJSON;
using UntoldTracks.Managers;

public class PlaceableEntity : Entity, ITokenizable
{
    public ItemModel source;
    public List<Transform> raycastOrigins = new();
    private Renderer[] _allRenderers;
    private Material[] _allMaterials;
    private bool _beingPlaced;
    private bool _isTriggering = false;

    private ColliderEvents _colliderEvents;
    public List<Collider> ignoreWhenPlacing = new();

    [SerializeField]
    private Transform _worldTransform;

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
        transform.SetParent(GameManager.Instance.TrainManager.Train.GetClosestCarriage(transform).transform, true);
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
            ignoreWhenPlacing.ForEach(x => x.gameObject.SetActive(false));
            gameObject.ChildCollidersToTriggers(true);
        }
        else
        {
            Utility.SetLayerRecursively(_worldTransform.gameObject, LayerMask.NameToLayer("Default"));
            ignoreWhenPlacing.ForEach(x => x.gameObject.SetActive(true));
            gameObject.ChildCollidersToTriggers(false);
        }
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
