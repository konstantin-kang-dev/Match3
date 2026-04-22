using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public enum PlayfieldItemType
{
    ItemMilk = 1,
    ItemApple = 2,
    ItemLemon = 3,
    ItemCoconut = 4,
    ItemBroccoli = 5,
}

[Serializable, CreateAssetMenu(fileName = "PlayfieldItemConfig", menuName = "GameContent/PlayfieldItemConfig")]
public class PlayfieldItemConfig: ScriptableObject
{
    public PlayfieldItemType ItemType;
    public float ScoreWeight;
    public Sprite Icon;
    public GameObject Prefab;
}
