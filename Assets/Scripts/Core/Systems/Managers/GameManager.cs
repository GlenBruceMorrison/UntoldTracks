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
    public class GameManager : MonoBehaviour
    {
        public PlayerManager playerPrefab;

        private static GameManager _instance;
        
        public PlayerManager _playerManager;
        public EntityManager _entityManager;
        public Train _trainManager;

        public LoadingManager loader;

        public static GameManager Instance => _instance;
        public EntityManager EntityManager => _entityManager;
        public PlayerManager LocalPlayer => _playerManager;
        public Train TrainManager => _trainManager;

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

            // load game data
            var gameData = loader.LoadGameData(this);

            // init player
            _playerManager = Instantiate(playerPrefab, new Vector3(gameData.player.posX, gameData.player.posY, gameData.player.posZ), Quaternion.identity);
            
            _playerManager.Build(gameData.player);
            _trainManager.Build(gameData.train);
        }
    }
}
