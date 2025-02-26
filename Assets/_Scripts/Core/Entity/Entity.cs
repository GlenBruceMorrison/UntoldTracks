﻿using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

public delegate void EntityMove(Vector3 old, Vector3 current);

public class Entity : MonoBehaviour, ITokenizable
{
    #region Token
    public virtual void Load(JSONNode node)
    {
        var position = node["position"].ReadVector3();
        var rotation = node["rotation"].ReadQuaternion();

        transform.position = position;
        transform.rotation = rotation;
    }

    public virtual JSONObject Save()
    {
        var entityJSON = new JSONObject();

        entityJSON.Add("position", new JSONObject().WriteVector3(transform.position));
        entityJSON.Add("rotation", new JSONObject().WriteQuaternion(transform.rotation));
        
        return entityJSON;
    }
    #endregion

    public virtual void Start()
    {

    }
}