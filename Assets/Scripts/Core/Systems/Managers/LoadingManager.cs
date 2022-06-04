using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UntoldTracks.Data;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Managers;

namespace UntoldTracks.Managers
{
    public class LoadingManager : MonoBehaviour
    {
        private string _path = "";
        private string _persistantPath = "";

        public ItemRegistry itemRegistry;


        public void SetPath()
        {
            // debug value to set path to out unity asset folder
            _path = Application.dataPath + Path.AltDirectorySeparatorChar + "GameData.json";

            // production path to set data to the actual computer
            _persistantPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "GameData.json";
        }

        public GameData LoadGameData(GameManager manager)
        {
            SetPath();
            try
            {
                using StreamReader reader = new StreamReader(_path);
                string json = reader.ReadToEnd();

                var gameData = JsonUtility.FromJson<GameData>(json);

                return gameData;
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
                using StreamWriter reader = new StreamWriter(_path);

                var gameData = new GameData();

                gameData.player = FindObjectOfType<PlayerManager>().Save();
                gameData.train = FindObjectOfType<Train>().Save();

                var json = JsonUtility.ToJson(gameData);

                reader.Write(json);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown("i"))
            {
                SaveGameData();
                Application.Quit();
            }
        }
    }
}