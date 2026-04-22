using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Game
{
    public class MatchDetector
    {
        readonly IBoard _board;

        static readonly Vector2Int[] Dirs4 =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        public MatchDetector(IBoard board)
        {
            _board = board;
        }

        public List<MatchGroup> FindMatches(IEnumerable<Vector2Int> changedCells)
        {
            var matched = CollectMatchedCells(changedCells);
            if (matched.Count == 0) return new List<MatchGroup>();
            return GroupIntoComponents(matched);
        }

        HashSet<Vector2Int> CollectMatchedCells(IEnumerable<Vector2Int> changedCells)
        {
            var matched = new HashSet<Vector2Int>();

            if (changedCells == null)
            {
                var size = _board.Size;
                for (int x = 0; x < size.x; x++)
                    for (int y = 0; y < size.y; y++)
                    {
                        var pos = new Vector2Int(x, y);
                        CheckLine(pos, Vector2Int.right, matched);
                        CheckLine(pos, Vector2Int.up, matched);
                    }
            }
            else
            {
                var toCheck = BuildCheckSet(changedCells);
                foreach (var pos in toCheck)
                {
                    CheckLine(pos, Vector2Int.right, matched);
                    CheckLine(pos, Vector2Int.up, matched);
                }
            }

            return matched;
        }

        List<MatchGroup> GroupIntoComponents(HashSet<Vector2Int> matched)
        {
            var groups = new List<MatchGroup>();
            var visited = new HashSet<Vector2Int>();
            var queue = new Queue<Vector2Int>();

            foreach (var start in matched)
            {
                if (visited.Contains(start)) continue;

                var type = _board.GetType(start);
                if (!type.HasValue) continue;

                var component = new List<Vector2Int>();
                queue.Clear();
                queue.Enqueue(start);
                visited.Add(start);

                while (queue.Count > 0)
                {
                    var cell = queue.Dequeue();
                    component.Add(cell);

                    foreach (var dir in Dirs4)
                    {
                        var neighbor = cell + dir;
                        if (!matched.Contains(neighbor)) continue;
                        if (visited.Contains(neighbor)) continue;
                        if (_board.GetType(neighbor) != type) continue;

                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }

                groups.Add(new MatchGroup(component, type.Value));
            }

            return groups;
        }

        HashSet<Vector2Int> BuildCheckSet(IEnumerable<Vector2Int> changedCells)
        {
            var set = new HashSet<Vector2Int>();
            var size = _board.Size;

            foreach (var cell in changedCells)
            {
                for (int x = 0; x < size.x; x++) TryAdd(set, new Vector2Int(x, cell.y));
                for (int y = 0; y < size.y; y++) TryAdd(set, new Vector2Int(cell.x, y));
            }

            return set;
        }

        void CheckLine(Vector2Int pos, Vector2Int dir, HashSet<Vector2Int> matched)
        {
            Vector2Int prev = pos - dir;
            if (_board.IsInBounds(prev))
            {
                var prevType = _board.GetType(prev);
                var curType = _board.GetType(pos);
                if (prevType.HasValue && curType.HasValue && prevType == curType) return;
            }

            var run = new List<Vector2Int>();
            Vector2Int current = pos;

            while (_board.IsInBounds(current))
            {
                var type = _board.GetType(current);
                if (!type.HasValue) break;
                if (run.Count > 0 && _board.GetType(run[0]) != type) break;

                run.Add(current);
                current += dir;
            }

            if (run.Count >= 3)
                foreach (var cell in run)
                    matched.Add(cell);
        }

        void TryAdd(HashSet<Vector2Int> set, Vector2Int pos)
        {
            if (_board.IsInBounds(pos)) set.Add(pos);
        }
    }
}