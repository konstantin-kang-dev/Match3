using Game;
using System;
using System.Collections.Generic;
using System.Text;
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
    public class MatchShapesConfig: ScriptableObject
    {
        Dictionary<MatchShape, MatchShapeData> _cachedData = new Dictionary<MatchShape, MatchShapeData>();
        public List<MatchShapeData> MatchShapesData = new List<MatchShapeData>();

        public void CacheData()
        {
            _cachedData.Clear();

            foreach (var matchGroupData in MatchShapesData)
            {
                _cachedData.Add(matchGroupData.Shape, matchGroupData);
            }
        }
        public float GetExpRewardByShapeType(MatchShape type)
        {
            if (!_cachedData.ContainsKey(type)) return 0f;

            return _cachedData[type].ExpReward;
        }
    }


}
