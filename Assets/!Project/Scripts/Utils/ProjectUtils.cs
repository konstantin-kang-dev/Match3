using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils
{
    public static class ProjectUtils
    {
        public static Vector2Int GetSwipeDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                return delta.x > 0 ? Vector2Int.right : Vector2Int.left;
            else
                return delta.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

        public static PlayfieldItemType GetRandomPlayfieldItemType()
        {
            var values = Enum.GetValues(typeof(PlayfieldItemType));
            var random = (PlayfieldItemType)values.GetValue(Random.Range(0, values.Length));
            return random;
        }
        public static PlayfieldItemType GetRandomPlayfieldItemTypeExcluding(HashSet<PlayfieldItemType> forbidden)
        {
            var available = System.Enum.GetValues(typeof(PlayfieldItemType))
                .Cast<PlayfieldItemType>()
                .Where(t => !forbidden.Contains(t))
                .ToList();

            return available[Random.Range(0, available.Count)];
        }
    }
}
