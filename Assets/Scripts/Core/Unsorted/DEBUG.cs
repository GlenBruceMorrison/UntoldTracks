using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UntoldTracks.Managers;
using UntoldTracks.Models;
using UntoldTracks.InventorySystem;
using PathCreation;

public class DEBUG : MonoBehaviour
{
    private List<ItemModel> models = new();
    private int toolbarInt = 0;
    private bool show = false;

    private readonly string[] toolbarStrings = { "Inventory", "Saving/Loading" };

    private void Awake()
    {
        models = ResourceService.Instance.GetAll<ItemModel>();
    }

    void OnGUI()
    {
        GUI.BeginGroup(new Rect(0, 0, 250, 900));

            show = GUILayout.Toggle(show, "Show");

            if (!show)
            {
                GUI.EndGroup();
                return;
            }

            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);

            GUI.BeginGroup(new Rect(0, 0, 250, 900));
                switch (toolbarInt)
                {
                    case 0:
                        for (var i = 0; i < models.Count; i++)
                        {
                            var item = models[i];

                            if (GUILayout.Button(item.displayName))
                            {
                                GameManager.Instance.LocalPlayer.InventoryController.Inventory.Give(new ItemContainer(item, 1));
                            }
                        }
                        break;
                    case 1:
                        if (GUILayout.Button("Reset Save"))
                        {
                            GameManager.Instance.Reset();
                        }

                        if (GUILayout.Button("Save"))
                        {
                            GameManager.Instance.SaveGameData();
                        }
                    break;
                }

            GUI.EndGroup();

        GUI.EndGroup();
    }
}
