
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BoardCollapser
    {
        readonly PlayfieldBoard _board;

        public BoardCollapser(PlayfieldBoard board)
        {
            _board = board;
        }

        public List<CellMovement> Collapse()
        {
            var size = _board.Size;
            var collapsedCells = new List<CellMovement>();

            for (int x = 0; x < size.x; x++)
            {
                int writeY = 0;
                for (int readY = 0; readY < size.y; readY++)
                {
                    if (_board.Get(new Vector2Int(x, readY)) == null) continue;
                    if (readY != writeY)
                    {
                        Vector2Int initialCell = new Vector2Int(x, readY);
                        Vector2Int targetCell = new Vector2Int(x, writeY);
                        var item = _board.Get(initialCell);
                        item.OccupyCell(targetCell);
                        _board.Clear(initialCell);
                        _board.Set(targetCell, item);

                        CellMovement cellMovement = new CellMovement(item, initialCell, targetCell, false);
                        collapsedCells.Add(cellMovement);
                    }
                    writeY++;
                }
            }
            
            return collapsedCells;

        }
    }
}
