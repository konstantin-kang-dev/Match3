using System;
using UnityEngine;

[Serializable]
public enum PlayfieldItemColorType
{
    ItemRed = 1,
    ItemGreen = 2,
    ItemYellow = 3,
    ItemPink = 4,
}

public enum PlayfieldItemKind
{
    Colored = 0,

    Rocket = 100,
    Bomb = 101,
    PaperPlane = 102,
    Disco = 103,
}

namespace Game.Configs
{
    public abstract class PlayfieldItemConfig : ScriptableObject
    {
        [SerializeField] PlayfieldItemKind _kind;
        [SerializeField] Sprite _icon;
        [SerializeField] GameObject _prefab;

        public PlayfieldItemKind Kind => _kind;
        public Sprite Icon => _icon;
        public GameObject Prefab => _prefab;
    }
}



