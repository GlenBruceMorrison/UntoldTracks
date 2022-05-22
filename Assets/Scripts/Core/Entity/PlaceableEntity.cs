using System;
using UnityEngine;
using System.Collections.Generic;

public class PlaceableEntity : Entity
{
    public Item source;
    public List<Transform> raycastOrigins = new List<Transform>();
    private Collider[] _worldColliders;

    [SerializeField]
    private PlacableEntityIndicator _placabableEntityIndicator;

    [SerializeField]
    private Transform _worldTransform;

    private bool _beingPlaced;
    private bool _isTriggering = false;
    private Material _origionalMaterial;
    private MeshRenderer _meshRenderer;
    private Renderer[] _allRenderers;
    private Material[] _allMaterials;

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
            _beingPlaced = value;
            
            if (_beingPlaced)
            {
                _placabableEntityIndicator.gameObject.SetActive(true);

                foreach (var collider in _worldColliders)
                {
                    collider.enabled = false;
                }
            }
            else
            {
                _placabableEntityIndicator.gameObject.SetActive(false);

                foreach (var collider in _worldColliders)
                {
                    collider.enabled = true;
                }
            }
        }
    }

    #region Unity
    void Awake()
    {
        GrabAllRenderers();
        _worldColliders = _worldTransform.GetComponentsInChildren<Collider>();
        _placabableEntityIndicator = GetComponentInChildren<PlacableEntityIndicator>();

        if (_placabableEntityIndicator == null)
        {
            Debug.LogWarning("This object does not have a PlaceableEntityIndicator on it, disabling!");
            this.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        _placabableEntityIndicator.TriggerStay.AddListener(HandleTriggerStay);
        _placabableEntityIndicator.TriggerExit.AddListener(HandleTriggerExit);
    }

    private void OnDisable()
    {
        _placabableEntityIndicator.TriggerStay.RemoveListener(HandleTriggerStay);
        _placabableEntityIndicator.TriggerExit.RemoveListener(HandleTriggerExit);
    }
    #endregion

    #region EventHandlers
    public void HandleTriggerStay()
    {
        _isTriggering = true;
    }

    public void HandleTriggerExit()
    {
        _isTriggering = false;
    }
    #endregion

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
}
