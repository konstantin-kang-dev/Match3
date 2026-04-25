using Game.Configs;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayfieldItemsDB : MonoBehaviour
{
    [SerializeField] string _configsPath = "PlayfieldItemsConfigs/";

    Dictionary<PlayfieldItemKind, PlayfieldItemConfig> _byKind = new();
    Dictionary<PlayfieldItemColorType, ColoredItemConfig> _byColor = new();

    public void Init()
    {
        _byKind.Clear();
        _byColor.Clear();

        var configs = Resources.LoadAll<PlayfieldItemConfig>(_configsPath);
        foreach (var config in configs)
        {
            if (config is ColoredItemConfig colored)
            {
                _byColor[colored.Color] = colored;
            }
            else
            {
                if (_byKind.ContainsKey(config.Kind))
                    Debug.LogError($"[PlayfieldItemsDB] Duplicate config for kind {config.Kind}");
                _byKind[config.Kind] = config;
            }
        }
    }

    public T Get<T>(PlayfieldItemKind kind) where T : PlayfieldItemConfig
    {
        if (!_byKind.TryGetValue(kind, out var config))
            throw new Exception($"[PlayfieldItemsDB] No config for kind {kind}");

        if (config is not T typed)
            throw new Exception($"[PlayfieldItemsDB] Config for {kind} is not {typeof(T).Name}");

        return typed;
    }

    public ColoredItemConfig GetColored(PlayfieldItemColorType color)
    {
        if (!_byColor.TryGetValue(color, out var config))
            throw new Exception($"[PlayfieldItemsDB] No colored config for {color}");
        return config;
    }
}