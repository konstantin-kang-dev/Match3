using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BoardCollapser
    {
        private readonly IBoard _board;

        public BoardCollapser(IBoard board)
        {
            _board = board;
        }

        public List<CellMovement> Collapse()
        {
            var size = _board.Size;
            var collapsedCells = new List<CellMovement>();

            for (var x = 0; x < size.x; x++)
            {
                var writeY = 0;
                for (var readY = 0; readY < size.y; readY++)
                {
                    if (_board.Get(new Vector2Int(x, readY)) == null) continue;
                    if (readY != writeY)
                    {
                        var initialCell = new Vector2Int(x, readY);
                        var targetCell = new Vector2Int(x, writeY);
                        var item = _board.Get(initialCell);
                        item.OccupyCell(targetCell);
                        _board.Clear(initialCell);
                        _board.Set(targetCell, item);

                        var cellMovement = new CellMovement(item, initialCell, targetCell, false);
                        collapsedCells.Add(cellMovement);
                    }

                    writeY++;
                }
            }

            return collapsedCells;
        }
    }
}