using System;
using System.Collections.Generic;
using Game;
using Game.Configs;
using UnityEngine;

public class PlayfieldVfxDB : MonoBehaviour
{
    [SerializeField] string _configsPath = "PlayfieldVfxConfigs/";
    readonly Dictionary<PlayfieldVfxType, PlayfieldVfxConfig> _byType = new();

    public void Init()
    {
        _byType.Clear();

        var configs = Resources.LoadAll<PlayfieldVfxConfig>(_configsPath);
        foreach (var config in configs)
        {
            if (_byType.ContainsKey(config.Type))
                Debug.LogError($"[PlayfieldVfxDB] Duplicate config for type {config.Type}");
            _byType[config.Type] = config;
        }
    }

    public PlayfieldVfxConfig Get(PlayfieldVfxType type)
    {
        if (!_byType.TryGetValue(type, out var config))
            throw new Exception($"[PlayfieldVfxDB] No config for type {type}");
        return config;
    }

    public bool TryGet(PlayfieldVfxType type, out PlayfieldVfxConfig config)
        => _byType.TryGetValue(type, out config);
}