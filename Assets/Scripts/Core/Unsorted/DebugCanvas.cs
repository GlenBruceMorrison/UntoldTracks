using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UntoldTracks.Player;

public class DebugCanvas : MonoBehaviour
{
    private TMP_Text console;

    private void Awake()
    {
        console = GetComponentInChildren<TMP_Text>();
    }

    private void Log(string text)
    {
        console.text = console.text + text + '\n';
    }

    private void Start()
    {
        var player = GameObject.FindObjectOfType<PlayerManager>();

        player.InventoryController.OnActiveItemChanged += (player, container) =>
        {
            if (container == null || container.Item == null)
            {
                return;
            }

            Log("Active item changed to " + ((container == null || container.Item == null) ? "Empty" : container.Item.name));
        };

        player.InventoryController.Inventory.OnContainerAdded += (container) =>
        {
            Log($"{container.Count} {container.Item.name}(s) added to player's inventory");
        };

        player.InventoryController.Inventory.OnContainerRemoved += (container) =>
        {
            Log($"{container.Count} {container.Item.name}(s) removed to player's inventory");
        };
    }
}
