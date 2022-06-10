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
    public class GameManager : MonoBehaviour, ITokenizable
    {
        private static GameManager _instance;
        [SerializeField] public SerializableRegistry _registry;
        private PlayerManager _playerManager;
        private TrainManager _trainManager;
        private LoadingManager _loader;

        public PlayerManager playerPrefab;
        public TrainManager trainPrefab;
        public UnityAction OnGameLoaded;

        public static GameManager Instance => _instance;
        public PlayerManager LocalPlayer => _playerManager;
        public TrainManager TrainManager => _trainManager;
        public SerializableRegistry Registry
        {
            get
            {
                if (_registry == null)
                {
                    Debug.LogError("REGISTRY IS NOT SET IN THE GAME MANAGER!");
                    Debug.LogError("CHECK IF THE REGISTRY IS IN A VALID STATE IN THE PROJECT DIRECTORY");
                    return null;
                }

                return _registry;
            }
        }

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
            // init player
            var playerPos = node["player"]["entity"]["position"].ReadVector3();
            _playerManager = Instantiate(playerPrefab, playerPos, Quaternion.identity);

            // init train
            _trainManager = Instantiate(trainPrefab, new Vector3(0, 2.28f, 0), Quaternion.identity);

            // load game
            _playerManager.Load(node["player"]);
            _trainManager.Load(node["train"]);

            OnGameLoaded?.Invoke();
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
