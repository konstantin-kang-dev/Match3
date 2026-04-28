using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;

namespace Game
{
    public class BoardFiller
    {
        private readonly IBoard _board;
        private readonly PlayfieldItemsFactory _factory;
        private readonly GridManager _gridManager;

        public BoardFiller(IBoard board, GridManager gridManager, PlayfieldItemsFactory factory)
        {
            _board = board;
            _gridManager = gridManager;
            _factory = factory;
        }

        public List<CellMovement> Refill()
        {
            var size = _board.Size;

            var cellMovements = new List<CellMovement>();

            for (var x = 0; x < size.x; x++)
            for (var y = 0; y < size.y; y++)
            {
                var cell = new Vector2Int(x, y);
                if (_board.Get(cell) != null) continue;

                var item = SpawnAt(cell);

                var startCell = new Vector2Int(cell.x, _gridManager.GridSize.y);
                var cellMovement = new CellMovement(item, startCell, cell, true);
                cellMovements.Add(cellMovement);
            }

            return cellMovements;
        }

        private PlayfieldItem SpawnAt(Vector2Int cell)
        {
            var isPowerUp = ProjectUtils.RollChance(ProjectConstants.BOARD_POWERUP_SPAWN_CHANCE);
            var itemKind = PlayfieldItemKind.Colored;
            var colorType = PlayfieldItemColorType.ItemGreen;

            PlayfieldItem item = null;
            if (isPowerUp)
            {
                itemKind = GetRandomTypePowerUp();
                switch (itemKind)
                {
                    case PlayfieldItemKind.Rocket:
                        item = _factory.SpawnRocket(RocketOrientation.Horizontal, _gridManager.PlayfieldItemsContainer);
                        break;
                    case PlayfieldItemKind.Bomb:
                        item = _factory.SpawnBomb(_gridManager.PlayfieldItemsContainer);
                        break;
                    case PlayfieldItemKind.Plane:
                        item = _factory.SpawnPlane(_gridManager.PlayfieldItemsContainer);
                        break;
                    case PlayfieldItemKind.Disco:
                        item = _factory.SpawnDisco(_gridManager.PlayfieldItemsContainer);
                        break;
                }
            }
            else
            {
                colorType = GetTypeWithoutMatch(cell.x, cell.y);
                item = _factory.SpawnColored(colorType, _gridManager.PlayfieldItemsContainer);
            }

            _board.Set(cell, item);
            item.OccupyCell(cell);
            item.Hide();

            return item;
        }

        private PlayfieldItemColorType GetTypeWithoutMatch(int x, int y)
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
                if (down1 != null && down2 != null && HaveSameColor(down1, down2))
                    forbidden.Add(down1.Color.Value);
            }

            if (x >= 1 && y >= 1)
            {
                var left = _board.Get(new Vector2Int(x - 1, y));
                var down = _board.Get(new Vector2Int(x, y - 1));
                var diag = _board.Get(new Vector2Int(x - 1, y - 1));
                if (HaveSameColor(left, down) && HaveSameColor(left, diag))
                    forbidden.Add(left.Color.Value);
            }

            return ProjectUtils.GetRandomPlayfieldItemColorTypeExcluding(forbidden);
        }

        private bool HaveSameColor(PlayfieldItem a, PlayfieldItem b)
        {
            return a != null && b != null
                             && a.Color.HasValue && b.Color.HasValue
                             && a.Color.Value == b.Color.Value;
        }

        private PlayfieldItemKind GetRandomTypePowerUp()
        {
            var forbidden = new HashSet<PlayfieldItemKind>();
            forbidden.Add(PlayfieldItemKind.Colored);

            return ProjectUtils.GetRandomPlayfieldItemKindExcluding(forbidden);
        }
    }
}