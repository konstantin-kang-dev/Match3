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

        public async UniTask SpawnInitial()
        {
            var size = _board.Size; 
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    SpawnAt(new Vector2Int(x, y));
                }

                await UniTask.WaitForSeconds(0.15f);
            }

        }

        public async UniTask Refill()
        {
            var size = _board.Size;

            var byTargetRow = new Dictionary<int, List<Vector2Int>>();

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var cell = new Vector2Int(x, y);
                    if (_board.Get(cell) != null) continue;

                    if (!byTargetRow.ContainsKey(y)) byTargetRow[y] = new();
                    byTargetRow[y].Add(cell);
                }
            }

            foreach (var kvp in byTargetRow.OrderBy(k => k.Key))
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.15f));
                foreach (var cell in kvp.Value)
                    SpawnAt(cell);

            }
        }

        void SpawnAt(Vector2Int cell)
        {
            var type = GetTypeWithoutMatch(cell.x, cell.y);
            var item = _factory.SpawnItem(type, _gridManager.PlayfieldItemsContainer);
            _board.Set(cell, item);

            Vector2 targetPos = _gridManager.GetPositionForCell(cell);

            Vector2Int startCell = new Vector2Int(cell.x, _gridManager.GridSize.y + 1);
            Vector2 startPos = _gridManager.GetPositionForCell(startCell);

            item.OccupyCell(cell, MoveAnimationType.None);
            item.View.MoveTo(startPos, MoveAnimationType.None);
            item.View.MoveTo(targetPos, MoveAnimationType.Bounce);
        }

        PlayfieldItemType GetTypeWithoutMatch(int x, int y)
        {
            var forbidden = new HashSet<PlayfieldItemType>();

            if (x >= 2)
            {
                var left1 = _board.Get(new Vector2Int(x - 1, y));
                var left2 = _board.Get(new Vector2Int(x - 2, y));
                if (left1 != null && left2 != null && left1.Model.Type == left2.Model.Type)
                    forbidden.Add(left1.Model.Type);
            }

            if (y >= 2)
            {
                var down1 = _board.Get(new Vector2Int(x, y - 1));
                var down2 = _board.Get(new Vector2Int(x, y - 2));
                if (down1 != null && down2 != null && down1.Model.Type == down2.Model.Type)
                    forbidden.Add(down1.Model.Type);
            }

            return ProjectUtils.GetRandomPlayfieldItemTypeExcluding(forbidden);
        }
    }
}