using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public delegate void EntityMove(Vector3 old, Vector3 current);

public class Entity : MonoBehaviour
{
    /*
    [System.Serializable]
    public class EntityData
    {
        public float posX, posY, posZ;
        public float rotX, rotY, rotZ;
        public float scaX, scaY, scaZ;
    }
    public EntityData entityData;

    public event EntityMove OnEntityMove;

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
        set
        {
            var oldPosition = Position;
            transform.position = value;
            OnEntityMove?.Invoke(oldPosition, Position);
        }
    }

    public float X
    {
        get
        {
            return transform.position.x;
        }
        set
        {
            ChangePostion(new Vector3(value, transform.position.y, transform.position.z));
        }
    }

    public float Y
    {
        get
        {
            return transform.position.y;
        }
        set
        {
            ChangePostion(new Vector3(transform.position.x, value, transform.position.z));
        }
    }

    public float Z
    {
        get
        {
            return transform.position.z;
        }
        set
        {
            ChangePostion(new Vector3(transform.position.x, transform.position.y, value));
        }
    }

    private void ChangePostion(Vector3 position)
    {
        var oldPosition = Position;
        transform.position = position;
        OnEntityMove?.Invoke(oldPosition, Position);
    }
    public virtual void LoadFromJson(string jsonString)
    {

    }
    
    public virtual void SaveToStream(StreamWriter writer)
    {
        var saveData = ToJson();
        writer.Write(saveData);
    }
    public virtual string ToJson()
    {
        EntityData data = new EntityData()
        {
            posX = X,
            posY = Y,
            posZ = Z
        };

        return JsonUtility.ToJson(data);
    }

    public virtual void Spawn()
    {

    }

    public virtual void DeSpawn()
    {

    }

    public virtual void Awake()
    {
        //FindObjectOfType<EntityManager>().entities.Add(this);
        //entityData = new EntityData()
        //{
        //    posX = X,
        //    posY = Y,
        //    posZ = Z
        //};
    }

    public object Tokenize()
    {
        return null;
    }

    public void LoadFromToken()
    {

    }
    */
}