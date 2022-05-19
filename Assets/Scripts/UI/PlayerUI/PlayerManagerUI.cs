using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UntoldTracks.Inventory;
using UntoldTracks.Player;

namespace UntoldTracks.UI
{
    public class PlayerManagerUI : MonoBehaviour
    {
        public PlayerManager player;

        private void Awake()
        {
            player = GetComponentInParent<PlayerManager>();
        }
    }
}