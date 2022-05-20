using System;
using UnityEngine;
using System.Collections.Generic;

public class PlaceableEntity : Entity
{
    public Item source;
    public List<Transform> raycastOrigins = new List<Transform>();
    
    private bool _beingPlaced;
    private int _startLayerIndex;

    private Material _origionalMaterial;
    
    private MeshRenderer _meshRenderer;
    
    public Material[] allMaterials;
 
    private Renderer[] _allRenderers;
    
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
                var layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                gameObject.layer = layerIgnoreRaycast;
            }
            else
            {
                gameObject.layer = _startLayerIndex;
            }
        }
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
            _allRenderers[i].material = allMaterials[i];
        }
    }

    public void GrabAllRenderers()
    {
        _allRenderers = GetComponentsInChildren<Renderer>();
        allMaterials = new Material[_allRenderers.Length];
        
        for (var i = 0; i < _allRenderers.Length; i++)
        {
            allMaterials[i] = _allRenderers[i].material;
        }
    }
    
    void Awake()
    {
        GrabAllRenderers();
        _startLayerIndex = gameObject.layer;
    }
}
