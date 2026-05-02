using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public readonly struct RecognizedShape
    {
        public readonly MatchShape Shape;
        public readonly IReadOnlyList<Vector2Int> ShapeCells;

        public RecognizedShape(MatchShape shape, IReadOnlyList<Vector2Int> shapeCells)
        {
            Shape = shape;
            ShapeCells = shapeCells;
        }
    }

    public static class MatchShapeRecognizer
    {
        public static List<MatchGroup> BuildGroups(List<MatchComponent> components)
        {
            var groups = new List<MatchGroup>(components.Count);
            foreach (var component in components)
            {
                var recognized = Recognize(component.Cells);
                if (!recognized.HasValue) continue;

                groups.Add(new MatchGroup(
                    cells: component.Cells,
                    shapeCells: recognized.Value.ShapeCells,
                    color: component.Color,
                    shape: recognized.Value.Shape));
            }
            return groups;
        }
        public static RecognizedShape? Recognize(IReadOnlyList<Vector2Int> componentCells)
        {
            if (componentCells.Count < 3) return null;

            var set = new HashSet<Vector2Int>(componentCells);

            if (TryFindLine(set, 5, out var line5, out _))
                return new RecognizedShape(MatchShape.Match5Line, line5);

            if (TryFindLT(set, out var lt))
                return new RecognizedShape(MatchShape.Match5LT, lt);

            if (TryFindSquare(set, out var square))
                return new RecognizedShape(MatchShape.Match4Square, square);

            if (TryFindLine(set, 4, out var line4, out var horiz4))
                return new RecognizedShape(
                    horiz4 ? MatchShape.Match4Horizontal : MatchShape.Match4Vertical,
                    line4);

            if (TryFindLine(set, 3, out var line3, out _))
                return new RecognizedShape(MatchShape.Match3, line3);

            return null;
        }

        static bool TryFindLine(
            HashSet<Vector2Int> cells,
            int minLength,
            out List<Vector2Int> result,
            out bool isHorizontal)
        {
            result = null;
            isHorizontal = false;

            List<Vector2Int> bestHorizontal = null;
            List<Vector2Int> bestVertical = null;

            foreach (var origin in cells)
            {
                if (!cells.Contains(origin + Vector2Int.left))
                {
                    var run = new List<Vector2Int> { origin };
                    var p = origin + Vector2Int.right;
                    while (cells.Contains(p)) { run.Add(p); p += Vector2Int.right; }
                    if (run.Count >= minLength &&
                        (bestHorizontal == null || run.Count > bestHorizontal.Count))
                        bestHorizontal = run;
                }

                if (!cells.Contains(origin + Vector2Int.down))
                {
                    var run = new List<Vector2Int> { origin };
                    var p = origin + Vector2Int.up;
                    while (cells.Contains(p)) { run.Add(p); p += Vector2Int.up; }
                    if (run.Count >= minLength &&
                        (bestVertical == null || run.Count > bestVertical.Count))
                        bestVertical = run;
                }
            }

            if (bestHorizontal == null && bestVertical == null) return false;

            if (bestHorizontal != null && (bestVertical == null || bestHorizontal.Count >= bestVertical.Count))
            {
                result = bestHorizontal;
                isHorizontal = true;
            }
            else
            {
                result = bestVertical;
                isHorizontal = false;
            }
            return true;
        }

        static bool TryFindLT(HashSet<Vector2Int> cells, out List<Vector2Int> result)
        {
            result = null;

            foreach (var pivot in cells)
            {
                int leftCount = 0;
                var p = pivot + Vector2Int.left;
                while (cells.Contains(p)) { leftCount++; p += Vector2Int.left; }
                int rightCount = 0;
                p = pivot + Vector2Int.right;
                while (cells.Contains(p)) { rightCount++; p += Vector2Int.right; }
                int hLen = leftCount + 1 + rightCount;

                int downCount = 0;
                p = pivot + Vector2Int.down;
                while (cells.Contains(p)) { downCount++; p += Vector2Int.down; }
                int upCount = 0;
                p = pivot + Vector2Int.up;
                while (cells.Contains(p)) { upCount++; p += Vector2Int.up; }
                int vLen = downCount + 1 + upCount;

                if (hLen < 3 || vLen < 3) continue;

                var shapeCells = new List<Vector2Int>(hLen + vLen - 1);
                for (int i = -leftCount; i <= rightCount; i++)
                    shapeCells.Add(new Vector2Int(pivot.x + i, pivot.y));
                for (int i = -downCount; i <= upCount; i++)
                {
                    if (i == 0) continue;
                    shapeCells.Add(new Vector2Int(pivot.x, pivot.y + i));
                }

                if (shapeCells.Count >= 5)
                {
                    result = shapeCells;
                    return true;
                }
            }

            return false;
        }

        static bool TryFindSquare(HashSet<Vector2Int> cells, out List<Vector2Int> result)
        {
            result = null;

            foreach (var c in cells)
            {
                var c10 = c + Vector2Int.right;
                var c01 = c + Vector2Int.up;
                var c11 = c + new Vector2Int(1, 1);

                if (cells.Contains(c10) && cells.Contains(c01) && cells.Contains(c11))
                {
                    result = new List<Vector2Int> { c, c10, c01, c11 };
                    return true;
                }
            }

            return false;
        }
    }
}