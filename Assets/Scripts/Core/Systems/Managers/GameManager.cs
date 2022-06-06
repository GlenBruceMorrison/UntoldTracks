using SimpleJSON;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UntoldTracks.Player;
using UntoldTracks.UI;

namespace UntoldTracks.Managers
{
    public class GameManager : MonoBehaviour, ITokenizable
    {
        public PlayerManager playerPrefab;

        private static GameManager _instance;

        [SerializeField] public SerializableRegistry _registry;
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private TrainManager _trainManager;
        [SerializeField] private LoadingManager _loader;

        public static GameManager Instance => _instance;
        public PlayerManager LocalPlayer => _playerManager;
        public TrainManager TrainManager => _trainManager;
        public SerializableRegistry Registry => _registry;

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

            Load(_loader.LoadData());
        }

        public void Load(JSONNode node)
        {
            // init player
            var playerPos = node["player"]["entity"]["position"].ReadVector3();
            _playerManager = Instantiate(playerPrefab, playerPos, Quaternion.identity);

            // load game
            _playerManager.Load(node["player"]);
            _trainManager.Load(node["train"]);
        }

        public JSONObject Save()
        {
            var root = new JSONObject();
            root.Add("player", _playerManager.Save());
            root.Add("train", _trainManager.Save());
            return root;
        }
    }
}
