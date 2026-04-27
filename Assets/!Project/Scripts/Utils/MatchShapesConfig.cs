using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

[Serializable]
public struct MatchShapeData
{
    public MatchShape Shape;
    public float ExpReward;
}

namespace Game
{
    [CreateAssetMenu(fileName = "MatchShapesConfig", menuName = "GameData/MatchShapesConfig")]
    public class MatchShapesConfig : ScriptableObject
    {
        public List<MatchShapeData> MatchShapesData = new();
        private readonly Dictionary<MatchShape, MatchShapeData> _cachedData = new();

        public void CacheData()
        {
            _cachedData.Clear();

            foreach (var matchGroupData in MatchShapesData) _cachedData.Add(matchGroupData.Shape, matchGroupData);
        }

        public float GetExpRewardByShapeType(MatchShape type)
        {
            if (!_cachedData.ContainsKey(type)) return 0f;

            return _cachedData[type].ExpReward;
        }
    }
}