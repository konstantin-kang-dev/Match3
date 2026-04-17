using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class PlayfieldItemsContentManager: MonoBehaviour
{
    [SerializeField] string _configsPath = "PlayfieldItemsConfigs/";
    Dictionary<PlayfieldItemType, PlayfieldItemConfig> _configs = new Dictionary<PlayfieldItemType, PlayfieldItemConfig>();

    public void Init()
    {
        LoadConfigs();
    }

    void LoadConfigs()
    {
        _configs.Clear();
        PlayfieldItemConfig[] configs = Resources.LoadAll<PlayfieldItemConfig>(_configsPath);

        foreach (var config in configs)
        {
            _configs.Add(config.ItemType, config);
        }
    }
}
