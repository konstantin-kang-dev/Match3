using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public enum PlayfieldItemType
{
    None = 0,
    CommonRed = 1,
    CommonBlue = 2,
    CommonGreen = 3,
    CommonPink = 4,
    CommonYellow = 5,
}

[Serializable, CreateAssetMenu(fileName = "PlayfieldItemConfig", menuName = "GameContent/PlayfieldItemConfig")]
public class PlayfieldItemConfig: ScriptableObject
{
    public PlayfieldItemType ItemType;
    public float ScoreWeight;
    public Sprite Icon;
}
