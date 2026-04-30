using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MatchDetector
    {
        private static readonly Vector2Int[] Dirs4 =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        readonly BoardState _board;

        public MatchDetector(BoardState board)
        {
            _board = board;
        }

        PlayfieldItemColorType? GetColor(Vector2Int pos)
        {
            var slot = _board.Get(pos);
            
            if (slot.State != CellState.Occupied) return null;
            return slot.Item?.Color;
        }

        bool IsInBounds(Vector2Int pos) => _board.IsInBounds(pos);

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
                var grid = _board.Size;
                for (int x = 0; x < grid.x; x++)
                for (int y = 0; y < grid.y; y++)
                {
                    var pos = new Vector2Int(x, y);
                    CheckLine(pos, Vector2Int.right, matched);
                    CheckLine(pos, Vector2Int.up, matched);
                    CheckSquare(pos, matched);
                }
            }
            else
            {
                var toCheck = BuildCheckSet(changedCells);
                foreach (var pos in toCheck)
                {
                    CheckLine(pos, Vector2Int.right, matched);
                    CheckLine(pos, Vector2Int.up, matched);
                    CheckSquare(pos, matched);
                }
            }

            return matched;
        }

        private List<MatchGroup> GroupIntoComponents(HashSet<Vector2Int> matched)
        {
            var groups = new List<MatchGroup>();
            var visited = new HashSet<Vector2Int>();
            var queue = new Queue<Vector2Int>();

            foreach (var start in matched)
            {
                if (visited.Contains(start)) continue;

                var type = GetColor(start);
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
                        if (GetColor(neighbor) != type) continue;

                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }

                groups.Add(new MatchGroup(component, type.Value));
            }

            return groups;
        }

        private HashSet<Vector2Int> BuildCheckSet(IEnumerable<Vector2Int> changedCells)
        {
            var set = new HashSet<Vector2Int>();
            var size = _board.Size;

            foreach (var cell in changedCells)
            {
                for (var x = 0; x < size.x; x++) TryAdd(set, new Vector2Int(x, cell.y));
                for (var y = 0; y < size.y; y++) TryAdd(set, new Vector2Int(cell.x, y));
            }

            return set;
        }

        private void CheckLine(Vector2Int pos, Vector2Int dir, HashSet<Vector2Int> matched)
        {
            var prev = pos - dir;
            if (_board.IsInBounds(prev))
            {
                var prevType = GetColor(prev);
                var curType = GetColor(pos);
                if (prevType.HasValue && curType.HasValue && prevType == curType) return;
            }

            var run = new List<Vector2Int>();
            var current = pos;

            while (_board.IsInBounds(current))
            {
                var type = GetColor(current);
                if (!type.HasValue) break;
                if (run.Count > 0 && GetColor(run[0]) != type) break;

                run.Add(current);
                current += dir;
            }

            if (run.Count >= 3)
                foreach (var cell in run)
                    matched.Add(cell);
        }
        
        void CheckSquare(Vector2Int pos, HashSet<Vector2Int> matched)
        {
            var c00 = pos;
            var c10 = pos + Vector2Int.right;
            var c01 = pos + Vector2Int.up;
            var c11 = pos + new Vector2Int(1, 1);

            if (!_board.IsInBounds(c10) || !_board.IsInBounds(c01) || !_board.IsInBounds(c11))
                return;

            var t00 = GetColor(c00);
            var t10 = GetColor(c10);
            var t01 = GetColor(c01);
            var t11 = GetColor(c11);

            if (!t00.HasValue) return;
            if (t00 != t10 || t00 != t01 || t00 != t11) return;

            matched.Add(c00);
            matched.Add(c10);
            matched.Add(c01);
            matched.Add(c11);
        }
        
        private void TryAdd(HashSet<Vector2Int> set, Vector2Int pos)
        {
            if (_board.IsInBounds(pos)) set.Add(pos);
        }
    }
}