using System;
using SimpleJSON;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks.Player;
using UntoldTracks.UI;

namespace UntoldTracks.Managers
{
    public static class App
    {
        public static GameManager MainManager
        {
            get
            {
                if (GameManager.Instance == null)
                {
                    throw new SystemException("Cannot find GameManager instance");
                }
                
                if (!GameManager.Instance.GameLoaded)
                {
                    throw new SystemException("Trying to access GameManager before the game has been loaded");
                }
                
                return GameManager.Instance;
            }
        }
        
        public static PlayerManager PlayerMananger => MainManager.LocalPlayer;
        public static TrainManager TrainManager => MainManager.TrainManager;
    }

    public class GameManager : MonoBehaviour, ITokenizable
    {
        public bool GameLoaded { get; private set; }
        
        private static GameManager _instance;
        private PlayerManager _playerManager;
        private TrainManager _trainManager;
        private LoadingManager _loader;

        public PlayerManager playerPrefab;
        public TrainManager trainPrefab;
        
        public static GameManager Instance => _instance;
        public PlayerManager LocalPlayer => _playerManager;
        public TrainManager TrainManager => _trainManager;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            _loader = GetComponent<LoadingManager>();

            Load(_loader.LoadData());
        }

        public void Reset()
        {
            _loader.ResetSave();
        }

        public void SaveGameData()
        {
            _loader.SaveGameData();
        }

        #region Token
        public void Load(JSONNode node)
        {
            // init train
            _trainManager = Instantiate(trainPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            
            // init player
            var playerPos = node["player"]["entity"]["position"].ReadVector3();
            _playerManager = Instantiate(playerPrefab, playerPos, Quaternion.identity);

            // load game
            _playerManager.Load(node["player"]);
            _trainManager.Load(node["train"]);

            GameLoaded = true;
        }

        public JSONObject Save()
        {
            var root = new JSONObject();
            root.Add("player", _playerManager.Save());
            root.Add("train", _trainManager.Save());
            return root;
        }
        #endregion
    }
}
