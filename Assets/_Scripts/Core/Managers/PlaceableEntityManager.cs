using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using UntoldTracks.Models;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Managers;
using SimpleJSON;

namespace UntoldTracks.Managers
{
    public class PlaceableEntityManager : MonoBehaviour, ITokenizable
    {
        public List<PlaceableEntity> entities = new();
        public List<ITokenizable> tokens = new();

        #region Token
        public void Load(JSONNode node)
        {
            foreach (var placeable in node["entities"].Children)
            {
                var entity = ResourceService.Instance.FindByGUID<ItemModel>(placeable["itemGUID"]);

                if (entity == null || entity.placeablePrefab == null)
                {
                    Debug.LogError($"Error when loading this entity. Check it exists in the registry and that the item GUID is correct.");
                    continue;
                }

                var position = placeable["position"].ReadVector3();

                var prefab = Instantiate(
                    entity.placeablePrefab,
                    position,
                    placeable["rotation"].ReadQuaternion());

                var token = prefab.GetComponent<ITokenizable>();

                if (token == null)
                {
                    Debug.LogError($"This prefab does not inherit from ITokenizable and so it cannot be loaded");
                    continue;
                }

                token.Load(placeable);

                entities.Add(prefab);
                tokens.Add(token);
            }

            foreach (var entity in entities)
            {
                entity.AfterBuild();
            }
        }

        public JSONObject Save()
        {
            var entitiesJSON = new JSONObject();
            var placeablesJSON = new JSONArray();

            foreach (var token in tokens)
            {
                placeablesJSON.Add(token.Save());
            }

            entitiesJSON.Add("entities", placeablesJSON);

            return entitiesJSON;
        }
        #endregion
    }
}