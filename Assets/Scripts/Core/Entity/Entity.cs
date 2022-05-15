using UnityEngine;

public delegate void EntityMove(Vector3 old, Vector3 current);

public class Entity : MonoBehaviour
{
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

}