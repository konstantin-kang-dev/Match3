using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProgressionConfig", menuName = "GameData/PlayerProgressionConfig")]
public class PlayerProgressionConfig: ScriptableObject
{
    public float StartRequiredExp = 100f;
    public float RequiredExpMultiplierPerLevel = 1.1f;

    public float GetRequiredExp(int level)
    {
        return StartRequiredExp * Mathf.Pow(RequiredExpMultiplierPerLevel, level - 1);
    }
}