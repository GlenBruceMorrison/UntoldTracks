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
    public List<Transform> raycastOrigins = new List<Transform>();

    private PlacableEntityIndicator _placeableEntityIndicator;
    private Collider[] _worldColliders;
    private Collider _rootCollider;
    private MeshRenderer _meshRenderer;
    private Renderer[] _allRenderers;
    private Material[] _allMaterials;
    private bool _beingPlaced;
    private bool _isTriggering = false;

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

    public override void Start()
    {
        base.Start();
    }

    public void AfterBuild()
    {
        transform.SetParent(
            GameManager.Instance.TrainManager.train.GetClosestCarriage(transform).transform,
            true);
    }

    protected virtual void OnEnable()
    {
        GrabAllRenderers();
        _worldColliders = _worldTransform.GetComponentsInChildren<Collider>();
        _placeableEntityIndicator = GetComponentInChildren<PlacableEntityIndicator>();
        _rootCollider = this.gameObject.GetComponent<Collider>();
        
        if (_placeableEntityIndicator == null)
        {
            Debug.LogWarning("This object does not have a PlaceableEntityIndicator on it, disabling!");
            this.gameObject.SetActive(false);
        }
        
        _placeableEntityIndicator.TriggerStay.AddListener(HandleTriggerStay);
        _placeableEntityIndicator.TriggerExit.AddListener(HandleTriggerExit);
    }

    protected virtual void OnDisable()
    {
        _placeableEntityIndicator.TriggerStay.RemoveListener(HandleTriggerStay);
        _placeableEntityIndicator.TriggerExit.RemoveListener(HandleTriggerExit);
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

    private void SetWorldColliders(bool state)
    {
        if (_rootCollider != null)
        {
            _rootCollider.enabled = state;
        }
                
        if (_worldColliders.Any())
        {
            foreach (var worldCollider in _worldColliders)
            {
                worldCollider.enabled = state;
            }
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
            _placeableEntityIndicator.gameObject.SetActive(true);
            SetWorldColliders(false);
        }
        else
        {
            _placeableEntityIndicator.gameObject.SetActive(false);
            SetWorldColliders(true);
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
