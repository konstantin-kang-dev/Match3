using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public enum PlayfieldItemColorType
{
    ItemRed = 1,
    ItemGreen = 2,
    ItemYellow = 3,
    ItemPink = 4
}

public enum PlayfieldItemKind
{
    Colored = 0,

    Rocket = 100,
    Bomb = 101,
    Balloon = 102,
    Disco = 103
}

namespace Game.Configs
{
    public abstract class PlayfieldItemConfig : SerializedScriptableObject
    {
        [SerializeField] private PlayfieldItemKind _kind;
        [SerializeField] private Sprite _icon;
        [SerializeField] private GameObject _prefab;

        public PlayfieldItemKind Kind => _kind;
        public Sprite Icon => _icon;
        public GameObject Prefab => _prefab;
    }
}