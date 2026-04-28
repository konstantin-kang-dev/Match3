using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class BoardContext : IBoardContext
    {
        private readonly IBoard _board;
        private readonly BoardMutator _mutator;
        private readonly GridManager _gridManager;

        public BoardContext(IBoard board, BoardMutator mutator, GridManager gridManager)
        {
            _board = board;
            _mutator = mutator;
            _gridManager = gridManager;
        }

        public Vector2Int Size => _board.Size;

        public bool IsValidCell(Vector2Int cell)
            => _board.IsInBounds(cell);

        public UniTask DestroyCells(IEnumerable<Vector2Int> cells, DestroyMode mode = DestroyMode.Animated, bool playVfx = true)
            => _mutator.DestroyCells(cells, this, mode, playVfx);
        
        public IEnumerable<Vector2Int> GetCellsInRadius(Vector2Int center, int radius)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    var cell = new Vector2Int(center.x + dx, center.y + dy);
                    if (IsValidCell(cell))
                        yield return cell;
                }
            }

        }
        public Vector2Int? FindRandomColoredCell()
        {
            var coloredCells = new List<Vector2Int>();
            for (int x = 0; x < _board.Size.x; x++)
            for (int y = 0; y < _board.Size.y; y++)
            {
                var cell = new Vector2Int(x, y);
                var item = _board.Get(cell);
                if (item != null && !item.IsPowerUp)
                    coloredCells.Add(cell);
            }

            if (coloredCells.Count == 0) return null;
            return coloredCells[Random.Range(0, coloredCells.Count)];
        }
        
        public IEnumerable<Vector2Int> GetCellsByColor(PlayfieldItemColorType color)
        {
            for (int x = 0; x < _board.Size.x; x++)
            for (int y = 0; y < _board.Size.y; y++)
            {
                var cell = new Vector2Int(x, y);
                var item = _board.Get(cell);
                if (item != null && item.Color == color)
                    yield return cell;
            }
        }
        
        public PlayfieldItemColorType? GetDominantColor()
        {
            var counts = new Dictionary<PlayfieldItemColorType, int>();
            for (int x = 0; x < _board.Size.x; x++)
            for (int y = 0; y < _board.Size.y; y++)
            {
                var item = _board.Get(new Vector2Int(x, y));
                if (item == null || !item.Color.HasValue) continue;

                var c = item.Color.Value;
                counts.TryGetValue(c, out int n);
                counts[c] = n + 1;
            }

            if (counts.Count == 0) return null;
            return counts.OrderByDescending(kvp => kvp.Value).First().Key;
        }
        
        public Vector2 GetWorldPosition(Vector2Int cell) => _gridManager.GetPositionForCell(cell);
    }
}