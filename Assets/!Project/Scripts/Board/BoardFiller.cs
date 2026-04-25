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
            bool isPowerUp = ProjectUtils.RollChance(ProjectConstants.BOARD_POWERUP_SPAWN_CHANCE);
            var itemKind = PlayfieldItemKind.Colored;
            var colorType = PlayfieldItemColorType.ItemGreen;

            PlayfieldItem item = null;
            if (isPowerUp)
            {
                itemKind = GetRandomTypePowerUp();
                item = _factory.SpawnPowerUp(itemKind, _gridManager.PlayfieldItemsContainer);
            }
            else
            {
                colorType = GetTypeWithoutMatch(cell.x, cell.y);
                item = _factory.SpawnColored(colorType, _gridManager.PlayfieldItemsContainer);
            }

            _board.Set(cell, item);
            item.OccupyCell(cell);

            return item;
        }

        PlayfieldItemColorType GetTypeWithoutMatch(int x, int y)
        {
            var forbidden = new HashSet<PlayfieldItemColorType>();

            if (x >= 2)
            {
                var left1 = _board.Get(new Vector2Int(x - 1, y));
                var left2 = _board.Get(new Vector2Int(x - 2, y));
                if (HaveSameColor(left1, left2))
                    forbidden.Add(left1.Color.Value);
            }

            if (y >= 2)
            {
                var down1 = _board.Get(new Vector2Int(x, y - 1));
                var down2 = _board.Get(new Vector2Int(x, y - 2));
                if (HaveSameColor(down1, down2))
                    forbidden.Add(down1.Color.Value);
            }

            return ProjectUtils.GetRandomPlayfieldItemColorTypeExcluding(forbidden);
        }
        bool HaveSameColor(PlayfieldItem a, PlayfieldItem b)
        {
            return a != null && b != null
                && a.Color.HasValue && b.Color.HasValue
                && a.Color.Value == b.Color.Value;
        }

        PlayfieldItemKind GetRandomTypePowerUp()
        {
            var forbidden = new HashSet<PlayfieldItemKind>();
            forbidden.Add(PlayfieldItemKind.Colored);

            return ProjectUtils.GetRandomPlayfieldItemKindExcluding(forbidden);
        }
    }
}