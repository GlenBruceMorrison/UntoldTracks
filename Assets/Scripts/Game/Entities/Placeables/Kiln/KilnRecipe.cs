using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks.Player;
using UntoldTracks.Models;
using UntoldTracks.Managers;

namespace UntoldTracks.Machines
{
    public class KilnRecipe : MonoBehaviour
    {
        public int cookingTimeRequired;
        
        public KilnObject input;
        public KilnObject output;

        public void Hide()
        {
            input.representation.gameObject.SetActive(false);
            output.representation.gameObject.SetActive(false);
        }

        public bool IsCooked(float cookingTime)
        {
            if (output.representation.gameObject.activeInHierarchy)
            {
                return true;
            }

            var cooked = cookingTime >= cookingTimeRequired;

            if (cooked == true)
            {
                input.representation.gameObject.SetActive(false);
                output.representation.gameObject.SetActive(true);
                return true;
            }

            return false;
        }
    }

    [System.Serializable]
    public class KilnObject
    {
        public Transform representation;
        public ItemModel model;
    }
}
