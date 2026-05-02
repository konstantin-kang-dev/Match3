using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum MatchShape
    {
        None,
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
        public readonly IReadOnlyList<Vector2Int> ShapeCells;
        public readonly PlayfieldItemColorType Color;
        public readonly MatchShape Shape;

        public MatchGroup(
            IReadOnlyList<Vector2Int> cells,
            IReadOnlyList<Vector2Int> shapeCells,
            PlayfieldItemColorType color,
            MatchShape shape)
        {
            Cells = cells;
            ShapeCells = shapeCells;
            Color = color;
            Shape = shape;
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
            PlayfieldItemColorType color,
            MatchShape shape,
            int cascadeLevel,
            Vector2 worldCenter)
        {
            Cells = cells;
            Color = color;
            Shape = shape;
            CascadeLevel = cascadeLevel;
            WorldCenter = worldCenter;
        }
    }
}