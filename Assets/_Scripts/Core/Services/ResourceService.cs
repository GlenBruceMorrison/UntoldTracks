using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceService : Singleton<ResourceService>
{
    [SerializeField] private List<SerializableScriptableObject> _assets;
    [SerializeField] private List<GeneratableBase> _generatables;

    public List<GeneratableBase> Generatables => _generatables;

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
        return _assets.OfType<T>().ToList();
    }

    public T GetRandomOfType<T>() where T : SerializableScriptableObject
    {
        var assets = GetAll<T>();
        return assets[Random.Range(0, assets.Count)];
    }

    protected override void Awake()
    {
        base.Awake();
        _assets = Resources.LoadAll<SerializableScriptableObject>("Data").ToList();
        _generatables = Resources.LoadAll<GeneratableBase>("Generation").ToList();
    }
}
