using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public enum PlayfieldItemType
{
    ItemRed = 1,
    ItemGreen = 2,
    ItemYellow = 3,
    ItemPink = 4,
}

[Serializable, CreateAssetMenu(fileName = "PlayfieldItemConfig", menuName = "GameData/PlayfieldItemConfig")]
public class PlayfieldItemConfig: ScriptableObject
{
    public PlayfieldItemType ItemType;
    public Sprite Icon;
    public GameObject Prefab;
}
