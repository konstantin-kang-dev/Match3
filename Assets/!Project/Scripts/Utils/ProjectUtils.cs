using Game;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils
{
    public static class ProjectUtils
    {
        public static bool RollChance(float chance)
        {
            return Random.Range(0, 100f) <= chance;
        }

        public static Vector2Int GetSwipeDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                return delta.x > 0 ? Vector2Int.right : Vector2Int.left;
            else
                return delta.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

        public static PlayfieldItemColorType GetRandomPlayfieldItemColorType()
        {
            var values = Enum.GetValues(typeof(PlayfieldItemColorType));
            var random = (PlayfieldItemColorType)values.GetValue(Random.Range(0, values.Length));
            return random;
        }
        public static PlayfieldItemColorType GetRandomPlayfieldItemColorTypeExcluding(HashSet<PlayfieldItemColorType> forbidden)
        {
            var available = System.Enum.GetValues(typeof(PlayfieldItemColorType))
                .Cast<PlayfieldItemColorType>()
                .Where(t => !forbidden.Contains(t))
                .ToList();

            return available[Random.Range(0, available.Count)];
        }
        public static PlayfieldItemKind GetRandomPlayfieldItemKindExcluding(HashSet<PlayfieldItemKind> forbidden)
        {
            var available = System.Enum.GetValues(typeof(PlayfieldItemKind))
                .Cast<PlayfieldItemKind>()
                .Where(t => !forbidden.Contains(t))
                .ToList();

            return available[Random.Range(0, available.Count)];
        }

        public static MatchShape Classify(List<Vector2Int> cells)
        {
            int count = cells.Count;

            int minX = cells.Min(c => c.x);
            int maxX = cells.Max(c => c.x);
            int minY = cells.Min(c => c.y);
            int maxY = cells.Max(c => c.y);

            int w = maxX - minX + 1;
            int h = maxY - minY + 1;

            // 1. Квадрат 2×2 — высший приоритет
            if (count == 4 && w == 2 && h == 2)
                return MatchShape.Match4Square;

            // 2. Чистая линия — bbox толщиной 1 И все клетки bbox заполнены
            bool isPureLine = (w == 1 || h == 1) && count == Mathf.Max(w, h);

            if (isPureLine)
            {
                bool isHorizontal = (h == 1);

                if (count == 3) return MatchShape.Match3;
                if (count == 4) return isHorizontal ? MatchShape.Match4Horizontal : MatchShape.Match4Vertical;
                if (count >= 5) return MatchShape.Match5Line;
            }

            // 3. Всё остальное — L/T/кластер → Bomb
            return MatchShape.Match5LT;
        }

        private static readonly string[] Suffixes = { "", "K", "M", "B", "T", "Qa", "Qi" };

        public static string FormatNumber(float value)
        {
            if (value < 1000) return value.ToString();

            int suffixIndex = 0;
            double shortValue = value;

            while (shortValue >= 1000 && suffixIndex < Suffixes.Length - 1)
            {
                shortValue /= 1000;
                suffixIndex++;
            }

            shortValue = Math.Floor(shortValue * 10) / 10;
            return shortValue.ToString("0.#", CultureInfo.InvariantCulture) + Suffixes[suffixIndex];
        }
    }
}
