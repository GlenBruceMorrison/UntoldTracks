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

            GUI.EndGroup();

        GUI.EndGroup();
    }
}
