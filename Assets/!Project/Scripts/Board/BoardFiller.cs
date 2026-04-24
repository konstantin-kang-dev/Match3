using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Game
{
    public class BoardFiller
    {
        readonly PlayfieldBoard _board;
        readonly GridManager _gridManager;
        readonly PlayfieldItemsFactory _factory;

        public BoardFiller(PlayfieldBoard board, GridManager gridManager, PlayfieldItemsFactory factory)
        {
            _board = board;
            _gridManager = gridManager;
            _factory = factory;
        }

        public List<CellMovement> Refill()
        {
            var size = _board.Size;

            List<CellMovement> cellMovements = new List<CellMovement>();

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var cell = new Vector2Int(x, y);
                    if (_board.Get(cell) != null) continue;

                    PlayfieldItem item = SpawnAt(cell);

                    Vector2Int startCell = new Vector2Int(cell.x, _gridManager.GridSize.y);
                    CellMovement cellMovement = new CellMovement(item, startCell, cell, true);
                    cellMovements.Add(cellMovement);
                }
            }

            return cellMovements;
        }

        PlayfieldItem SpawnAt(Vector2Int cell)
        {
            var type = GetTypeWithoutMatch(cell.x, cell.y);
            var item = _factory.SpawnItem(type, _gridManager.PlayfieldItemsContainer);
            _board.Set(cell, item);
            item.OccupyCell(cell);

            return item;
        }

        PlayfieldItemType GetTypeWithoutMatch(int x, int y)
        {
            var forbidden = new HashSet<PlayfieldItemType>();

            if (x >= 2)
            {
                var left1 = _board.Get(new Vector2Int(x - 1, y));
                var left2 = _board.Get(new Vector2Int(x - 2, y));
                if (left1 != null && left2 != null && left1.Type == left2.Type)
                    forbidden.Add(left1.Type);
            }

            if (y >= 2)
            {
                var down1 = _board.Get(new Vector2Int(x, y - 1));
                var down2 = _board.Get(new Vector2Int(x, y - 2));
                if (down1 != null && down2 != null && down1.Type == down2.Type)
                    forbidden.Add(down1.Type);
            }

            return ProjectUtils.GetRandomPlayfieldItemTypeExcluding(forbidden);
        }
    }
}