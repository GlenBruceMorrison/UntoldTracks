using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UntoldTracks.Managers;

namespace UntoldTracks.Player
{
    public class PlayerBuildingController : MonoBehaviour
    {
        [HideInInspector] public PlayerManager playerManager;
        [SerializeField] private bool _buildModeActive;

        public FoundationPiece buildIndicatorHolder;

        public GameObject testPrefab;

        public bool BuildModeActive
        {
            get
            {
                return _buildModeActive;
            }
        }

        private void Start()
        {
            //ActiveItemChangeEvent.RegisterListener(CheckForBuildMode);
        }

        private void OnDisable()
        {
            //ActiveItemChangeEvent.UnregisterListener(CheckForBuildMode);
        }

        /*
        public void CheckForBuildMode(ActiveItemChangeEvent eventData)
        {
            Debug.Log(eventData.player.gameObject.name);

            if (eventData.player != playerManager)
            {
                return;
            }

            if (eventData.item.IsEmpty())
            {
                DeActivateBuildMode();
                return;
            }

            if (eventData.item.item.isBuildingTool)
            {
                ActivateBuildMode();
            }
            else
            {
                DeActivateBuildMode();
            }
        }
        */
        public void ActivateBuildMode()
        {
            _buildModeActive = true;
        }

        public void DeActivateBuildMode()
        {
            _buildModeActive = false;
        }

        public void MoveBuildObject(Vector3 pos)
        {
            if (!BuildModeActive)
            {
                testPrefab.transform.position = Vector3.one * 999;
                return;
            }

            if (pos == Vector3.zero)
            {
                testPrefab.transform.position = Vector3.one * 999;
                return;
            }

            testPrefab.transform.position = pos;
        }
    }
}