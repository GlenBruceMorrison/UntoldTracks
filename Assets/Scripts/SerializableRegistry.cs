using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu]
public class SerializableRegistry : ScriptableObject
{
    [SerializeField] private SerializableScriptableObject[] _assets;

    public T FindByGUID<T>(string guid) where T : SerializableScriptableObject
    {
        return _assets.FirstOrDefault(x => x.Guid == guid) as T;
    }

    public T FirstOfType<T>() where T : SerializableScriptableObject
    {
        return _assets.FirstOrDefault(x => x is T) as T;
    }

    public List<T> GetAll<T>() where T : SerializableScriptableObject
    {
        return _assets.Where(x => x is T).ToList() as List<T>;
    }
}