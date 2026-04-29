using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class BoardContext : IBoardContext
    {
        private readonly BoardState _board;
        private readonly BoardMutator _mutator;
        private readonly GridManager _gridManager;

        public BoardContext(BoardState board, BoardMutator mutator, GridManager gridManager)
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
            foreach (var slot in _board.AllCells())
            {
                if (!IsTargetable(slot)) continue;
                var item = slot.Item;
                if (item != null && !item.IsPowerUp)
                    coloredCells.Add(slot.Position);
            }

            if (coloredCells.Count == 0) return null;
            return coloredCells[Random.Range(0, coloredCells.Count)];
        }

        public IEnumerable<Vector2Int> GetCellsByColor(PlayfieldItemColorType color)
        {
            foreach (var slot in _board.AllCells())
            {
                if (!IsTargetable(slot)) continue;
                var item = slot.Item;
                if (item != null && item.Color == color)
                    yield return slot.Position;
            }
        }

        public PlayfieldItemColorType? GetDominantColor()
        {
            var counts = new Dictionary<PlayfieldItemColorType, int>();
            foreach (var slot in _board.AllCells())
            {
                if (!IsTargetable(slot)) continue;
                var item = slot.Item;
                if (item == null || !item.Color.HasValue) continue;

                var c = item.Color.Value;
                counts.TryGetValue(c, out int n);
                counts[c] = n + 1;
            }

            if (counts.Count == 0) return null;
            return counts.OrderByDescending(kvp => kvp.Value).First().Key;
        }

        public Vector2 GetWorldPosition(Vector2Int cell) => _gridManager.GetPositionForCell(cell);

        // Клетка является валидной целью для PowerUp'а, если в ней реально стоит/летит фишка,
        // и она не находится в процессе уничтожения. Empty / Destroying — невалидны.
        static bool IsTargetable(CellSlot slot)
            => slot.State == CellState.Occupied || slot.State == CellState.Falling;
    }
}