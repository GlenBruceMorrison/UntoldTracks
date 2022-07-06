using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UntoldTracks.Managers;
using UntoldTracks.Models;
using UntoldTracks.InventorySystem;
using PathCreation;

public class DEBUG : MonoBehaviour
{
    private class GUIBool
    {
        private bool _val;
        private bool _old;
        public bool Value
        {
            get
            {
                return _val;
            }
            set
            {
                _old = _val;
                _val = value;
            }
        }

        public void Evaluate(Action<bool> action)
        {
            if (_val != _old)
            {
                action(_val);
            }
        }
    }
    
    private class GUIFloat
    {
        private float _val;
        private float _old;
        public float Value
        {
            get
            {
                return _val;
            }
            set
            {
                _old = _val;
                _val = value;
            }
        }

        public void Evaluate(Action<float> action)
        {
            if (_val != _old)
            {
                action(_val);
            }
        }
    }
    
    private List<ItemModel> models = new();
    private int toolbarInt = 0;
    private bool show = false;

    private Vector2 scrollPosition;

    private GUIBool limitPlacingToTrain = new();
    private GUIBool trainIsMoving = new();
    private GUIFloat trainSpeed = new();
    private GUIFloat trainTravelled = new();
    
    private readonly string[] toolbarStrings = { "Inventory", "Saving/Loading", "Train" };

    private void Awake()
    {
        models = ResourceService.Instance.GetAll<ItemModel>();
    }

    void OnGUI()
    {
        show = GUILayout.Toggle(show, "Show");

        if (!show)
        {
            return;
        }
        
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);
        
        
        int main_width = 250;
        int main_height = 500;
        int box_dimensions = 45;
        
        GUI.Box(new Rect(0, 45, main_width, main_height + 45), "");
        
        GUI.BeginGroup(new Rect(0, 0, main_width, main_height+100));
        
        scrollPosition = GUILayout.BeginScrollView(
            scrollPosition, 
            false, 
            true,  
            GUILayout.Width(main_width),  GUILayout.Height(main_height));
        
        switch (toolbarInt)
        {
            case 0:
                GUILayout.BeginVertical();
                
                for (var i = 0; i < models.Count; i++)
                {
                    var item = models[i];

                    if (item == null || item.sprite == null || item.sprite.texture == null)
                    {
                        continue;
                    }
                    
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(item.sprite.texture, 
                            GUILayout.MaxHeight(box_dimensions),
                            GUILayout.MaxWidth(box_dimensions)))
                    {
                        GameManager.Instance.LocalPlayer.InventoryController.Inventory.Give(new ItemContainer(item, 1));
                    }
                    
                    GUILayout.Label(item.displayName);
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
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
            case 2:
                if (GUILayout.Button("Extend Track"))
                {
                    GameManager.Instance.TrainManager.TrackGenerator.ExtendTrack();
                }

                if (GUILayout.Button("Spawn Island"))
                {
                    FindObjectOfType<WorldManager>().SpawnGeneratable();
                }

                limitPlacingToTrain.Value = GUILayout.Toggle(limitPlacingToTrain.Value, "Limit Placing To Train");
                limitPlacingToTrain.Evaluate((val) => FindObjectOfType<PlaceableEntityController>().limitPlacingToCarriage = val);
                
                trainIsMoving.Value = GUILayout.Toggle(trainIsMoving.Value, "Train Stop/Start");
                trainIsMoving.Evaluate((val) =>
                {
                    if (val)
                        FindObjectOfType<Train>().Move();
                    else
                        FindObjectOfType<Train>().Brake();
                });
                
                GUILayout.Label("Train Speed");
                trainSpeed.Value = GUILayout.HorizontalSlider(trainSpeed.Value, 0, 100);
                trainSpeed.Evaluate((val) => FindObjectOfType<Train>().CurrentSpeed = val);
                
                GUILayout.Label("Train Traveled");
                trainTravelled.Value = GUILayout.HorizontalSlider(trainTravelled.Value, 0, 9999);
                trainTravelled.Evaluate((val) => FindObjectOfType<Train>().DistanceTravelled = val);
                break;
        }
        GUILayout.EndScrollView();
        GUI.EndGroup();
    }
}
