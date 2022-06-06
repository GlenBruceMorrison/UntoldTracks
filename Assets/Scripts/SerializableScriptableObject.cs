using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SerializableScriptableObject : ScriptableObject
{
    [SerializeField] string _guid;
    public string Guid => _guid;

#if UNITY_EDITOR
    void OnValidate()
    {
        var path = AssetDatabase.GetAssetPath(this);
        _guid = AssetDatabase.AssetPathToGUID(path).ToString();
    }
#endif
}

[CreateAssetMenu]
public class CarriageDD : SerializableScriptableObjectPrefab<Carriage>
{

}

public class SerializableScriptableObjectPrefab<T> : SerializableScriptableObject where T : MonoBehaviour
{
    public T prefab;

    public T GetPrefab<T>(SerializableRegistry registry) where T : SerializableScriptableObject
    {
        return registry.FindByGUID<T>(Guid);
    }
}