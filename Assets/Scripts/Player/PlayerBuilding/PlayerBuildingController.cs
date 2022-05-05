using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Tracks.Systems.Building;

public class PlayerBuildingController : MonoBehaviour
{
    [HideInInspector]
    public PlayerManager playerManager;

    [SerializeField]
    private bool _buildModeActive;


    public FoundationPiece buildIndicatorHolder;

    public GameObject testPrefab;

    public bool BuildModeActive
    {
        get
        {
            return _buildModeActive;
        }
    }

    private void Start()
    {
        var inv = playerManager.inventoryController;
        playerManager.inventoryController.InventoryBar.onActiveItemChanged += CheckForBuildMode;
    }

    private void OnDisable()
    {
        playerManager.inventoryController.InventoryBar.onActiveItemChanged -= CheckForBuildMode;
    }

    public void CheckForBuildMode(ItemContainer itemContainer)
    {
        if (itemContainer.IsEmpty())
        {
            DeActivateBuildMode();
            return;
        }

        if (itemContainer.item.isBuildingTool)
        {
            ActivateBuildMode();
        }
        else
        {
            DeActivateBuildMode();
        }
    }

    public void ActivateBuildMode()
    {
        _buildModeActive = true;
    }

    public void DeActivateBuildMode()
    {
        _buildModeActive = false;
    }
    
    public void MoveBuildObject(Vector3 pos)
    {
        if (!BuildModeActive)
        {
            testPrefab.transform.position = Vector3.one*999;
            return;
        }

        if (pos == Vector3.zero)
        {
            testPrefab.transform.position = Vector3.one * 999;
            return;
        }

        testPrefab.transform.position = pos;
    }
}