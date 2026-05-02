using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Utils
{
    public static class ProjectUtils
    {
        private static readonly string[] Suffixes = { "", "K", "M", "B", "T", "Qa", "Qi" };

        public static bool RollChance(float chance)
        {
            return Random.Range(0, 100f) <= chance;
        }

        public static Vector2Int GetSwipeDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                return delta.x > 0 ? Vector2Int.right : Vector2Int.left;
            return delta.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

        public static PlayfieldVfxType ConvertColorTypeToVfxType(PlayfieldItemColorType colorType) => colorType switch
        {
            PlayfieldItemColorType.ItemRed => PlayfieldVfxType.MatchRedDestroy,
            PlayfieldItemColorType.ItemGreen => PlayfieldVfxType.MatchGreenDestroy,
            PlayfieldItemColorType.ItemYellow => PlayfieldVfxType.MatchYellowDestroy,
            PlayfieldItemColorType.ItemPink => PlayfieldVfxType.MatchPinkDestroy,
            _ => PlayfieldVfxType.MatchDestroy
        };
        
        public static PlayfieldItemColorType GetRandomPlayfieldItemColorType()
        {
            var values = Enum.GetValues(typeof(PlayfieldItemColorType));
            var random = (PlayfieldItemColorType)values.GetValue(Random.Range(0, values.Length));
            return random;
        }

        public static PlayfieldItemColorType GetRandomPlayfieldItemColorTypeExcluding(
            HashSet<PlayfieldItemColorType> forbidden)
        {
            var available = Enum.GetValues(typeof(PlayfieldItemColorType))
                .Cast<PlayfieldItemColorType>()
                .Where(t => !forbidden.Contains(t))
                .ToList();

            return available[Random.Range(0, available.Count)];
        }

        public static PlayfieldItemKind GetRandomPlayfieldItemKindExcluding(HashSet<PlayfieldItemKind> forbidden)
        {
            var available = Enum.GetValues(typeof(PlayfieldItemKind))
                .Cast<PlayfieldItemKind>()
                .Where(t => !forbidden.Contains(t))
                .ToList();

            return available[Random.Range(0, available.Count)];
        }
        

        public static string FormatNumber(float value)
        {
            if (value < 1000) return value.ToString();

            var suffixIndex = 0;
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