using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalSettingsService: Singleton<GlobalSettingsService>
{
    [SerializeField] private List<GlobalSettingsBase> _assets;
   
    public T FirstOfType<T>() where T : GlobalSettingsBase
    {
        return _assets.FirstOrDefault(x => x is T) as T;
    }

    protected override void Awake()
    {
        base.Awake();
        _assets = Resources.LoadAll<GlobalSettingsBase>("GlobalSettings").ToList();
    }
}
