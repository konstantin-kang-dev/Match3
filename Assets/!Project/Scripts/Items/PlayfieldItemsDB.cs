using System;
using System.Collections.Generic;
using Game.Configs;
using UnityEngine;

public class PlayfieldItemsDB : MonoBehaviour
{
    [SerializeField] private string _configsPath = "PlayfieldItemsConfigs/";
    private readonly Dictionary<PlayfieldItemColorType, ColoredItemConfig> _byColor = new();

    private readonly Dictionary<PlayfieldItemKind, PlayfieldItemConfig> _byKind = new();

    public void Init()
    {
        _byKind.Clear();
        _byColor.Clear();

        var configs = Resources.LoadAll<PlayfieldItemConfig>(_configsPath);
        foreach (var config in configs)
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