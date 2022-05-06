using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveItemChangeEvent : PlayerEvent<ActiveItemChangeEvent>
{
    public ItemContainer item;
}

public interface IEntity
{
    public float Y { get; }
    public float X { get; }
    public float Z { get; }
    public Vector3 Position { get; }

}