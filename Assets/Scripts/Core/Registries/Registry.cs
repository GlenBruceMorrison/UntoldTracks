using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Registry<T> : ScriptableObject where T : SerializableScriptableObject
{
    [SerializeField] protected List<T> _descriptors = new List<T>();

    public T FindByGUID(string guid)
    {
        return _descriptors.FirstOrDefault(x => x.Guid == guid);
    }
}