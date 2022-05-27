using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlaceableEntityConfig : MonoBehaviour
{
    private void Start()
    {
        Create();
    }

    public void Create()
    {
        // Root
        var root = new GameObject("PlaceableEntity");
        var rootBody = root.AddComponent<Rigidbody>();
        rootBody.isKinematic = true;
        var rootCollider = root.AddComponent<BoxCollider>();
        var rootScript = root.AddComponent<PlaceableEntity>();

        // Indicator
        var placableIndicator = new GameObject("PlacableIndicator");
        var placableriggger = placableIndicator.AddComponent<BoxCollider>();
        placableriggger.isTrigger = true;
        placableIndicator.layer = 2;
        placableIndicator.AddComponent<PlacableEntityIndicator>();
        placableriggger.transform.parent = root.transform;

        // Raycasts
        var placementRaycast = new GameObject("PlacableRaycasts");
        placementRaycast.transform.parent = root.transform;

        // WorldObject
        var placableWorldObject = new GameObject("PlacableWorldObject");
        placableWorldObject.transform.parent = root.transform;
        
        
    }
}
