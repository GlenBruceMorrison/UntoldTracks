using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UntoldTracks.Data;
using SimpleJSON;

public delegate void EntityMove(Vector3 old, Vector3 current);

public class Entity : MonoBehaviour, ITokenizable
{
    public virtual void Load(JSONNode node)
    {
        var position = node["entity"]["position"].ReadVector3();
        var rotation = node["entity"]["rotation"].ReadVector3();

        transform.position = position;
        transform.eulerAngles = rotation;
    }

    public virtual JSONObject Save()
    {
        var entityJSON = new JSONObject();

        var positionJSON = new JSONObject();
        positionJSON.Add("x", transform.position.x);
        positionJSON.Add("y", transform.position.y);
        positionJSON.Add("z", transform.position.z);

        var rotationJSON = new JSONObject();
        rotationJSON.Add("x", transform.rotation.x);
        rotationJSON.Add("y", transform.rotation.y);
        rotationJSON.Add("z", transform.rotation.z);

        entityJSON.Add("position", positionJSON);
        entityJSON.Add("rotation", rotationJSON);
        
        return entityJSON;
    }
}