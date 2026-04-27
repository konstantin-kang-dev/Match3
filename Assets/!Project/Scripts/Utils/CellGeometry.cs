using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

namespace Game.Utils
{
    public static class CellGeometry
    {
        public static Vector2Int GetIntersection(IReadOnlyList<Vector2Int> cells)
        {
            var set = new HashSet<Vector2Int>(cells);
            Vector2Int best = cells[0];
            int bestDegree = -1;
            foreach (var c in cells)
            {
                int degree = 0;
                if (set.Contains(c + Vector2Int.up)) degree++;
                if (set.Contains(c + Vector2Int.down)) degree++;
                if (set.Contains(c + Vector2Int.left)) degree++;
                if (set.Contains(c + Vector2Int.right)) degree++;
                if (degree > bestDegree) { bestDegree = degree; best = c; }
            }
            return best;
        }

        public static Vector2Int GetBottomLeft(IReadOnlyList<Vector2Int> cells)
            => new(cells.Min(c => c.x), cells.Min(c => c.y));

        public static Vector2Int GetCenter(IReadOnlyList<Vector2Int> cells)
            => cells[cells.Count / 2];
    }
}