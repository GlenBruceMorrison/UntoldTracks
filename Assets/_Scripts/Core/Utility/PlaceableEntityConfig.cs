using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UntoldTracks.Models;

public class PlaceableEntityConfig : MonoBehaviour
{
    [MenuItem("UntoldTracks/CreatePlacableItem")]
    public void CreatePlaceableItem()
    {
        // MyClass is inheritant from ScriptableObject base class
        var example = ScriptableObject.CreateInstance<ItemModel>();
        example.isPlaceable = true;

        // path has to start at "Assets"
        string path = $"Assets/Data/Items/test.asset";
        AssetDatabase.CreateAsset(example, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = example;
    }

    [MenuItem("UntoldTracks/CreatePlaceableEntity")]
    static void Create()
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
        placableIndicator.AddComponent<ColliderEvents>();
        placableriggger.transform.parent = root.transform;

        // Raycasts
        var placementRaycast = new GameObject("PlacableRaycasts");
        placementRaycast.transform.parent = root.transform;

        // WorldObject
        var placableWorldObject = new GameObject("PlacableWorldObject");
        placableWorldObject.transform.parent = root.transform;
    }
}
