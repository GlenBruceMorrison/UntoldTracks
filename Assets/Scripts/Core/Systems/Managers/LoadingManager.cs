using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UntoldTracks.Data;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Managers;
using SimpleJSON;
using static SimpleJSON.JSONNode;
using System.Text;

public interface ITokenizable
{
    void Load(JSONNode node);
    JSONObject Save();
}

namespace UntoldTracks.Managers
{

    public class LoadingManager : MonoBehaviour
    {
        private string _path = "";
        private string _persistantPath = "";

        private string _resetPath = "";

        public void SetPath()
        {
            // debug value to set path to out unity asset folder for reset save file
            _resetPath = Application.dataPath + Path.AltDirectorySeparatorChar + "GameDataReset.json";

            // debug value to set path to out unity asset folder
            _path = Application.dataPath + Path.AltDirectorySeparatorChar + "GameData.json";

            // production path to set data to the actual computer
            _persistantPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "GameData.json";
        }

        public JSONNode LoadData()
        {
            SetPath();
            try
            {
                using StreamReader reader = new StreamReader(_path);
                string json = reader.ReadToEnd();
                var root = JSON.Parse(json);

                return root;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            return null;
        }

        public void SaveGameData()
        {
            try
            {
                using StreamWriter writer = new(_path);
                var builder = new StringBuilder();
                var data = GameManager.Instance.Save();
                data.WriteToStringBuilder(builder, 0, 0, JSONTextMode.Indent);
                writer.Write(data.ToString());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void ResetSave()
        {
            try
            {
                using StreamReader reader = new(_resetPath);
                string jsonReset = reader.ReadToEnd();

                using StreamWriter writer = new(_path);
                writer.Write(jsonReset);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                SaveGameData();
            }
        }
    }
}