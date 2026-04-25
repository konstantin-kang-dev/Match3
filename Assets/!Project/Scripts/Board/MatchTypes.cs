using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Game
{
    public enum MatchShape
    {
        Match3,
        Match4Horizontal,
        Match4Vertical,
        Match4Square,
        Match5Line,
        Match5LT
    }

    public readonly struct MatchGroup
    {
        public readonly IReadOnlyList<Vector2Int> Cells;
        public readonly PlayfieldItemColorType Color;
        public readonly MatchShape Shape;

        public MatchGroup(List<Vector2Int> cells, PlayfieldItemColorType type)
        {
            Cells = cells;
            Color = type;
            Shape = ProjectUtils.Classify(cells);
        }
    }

    public readonly struct MatchResolvedEvent
    {
        public readonly IReadOnlyList<Vector2Int> Cells;
        public readonly PlayfieldItemColorType Color;
        public readonly MatchShape Shape;
        public readonly int CascadeLevel;
        public readonly Vector2 WorldCenter;

        public MatchResolvedEvent(
            IReadOnlyList<Vector2Int> cells,
            PlayfieldItemColorType type,
            MatchShape shape,
            int cascadeLevel,
            Vector2 worldCenter)
        {
            Cells = cells;
            Color = type;
            Shape = shape;
            CascadeLevel = cascadeLevel;
            WorldCenter = worldCenter;
        }
    }
}
