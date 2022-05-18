using System;
using UnityEngine;
using System.Collections.Generic;

public class PlaceableEntity : Entity
{
    public Item source;
    public List<Transform> raycastOrigins = new List<Transform>();
    
    private bool _beingPlaced;
    private int _startLayerIndex;
    
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
                int layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                gameObject.layer = layerIgnoreRaycast;
            }
            else
            {
                gameObject.layer = _startLayerIndex;
            }
        }
    }

    private void Awake()
    {
        _startLayerIndex = gameObject.layer;
    }

}
