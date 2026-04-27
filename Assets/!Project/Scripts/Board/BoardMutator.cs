using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Game.Utils;
using UnityEngine;

namespace Game
{
    public class BoardMutator
    {
        readonly PlayfieldBoard _board;
        readonly PlayfieldItemsFactory _factory;
        readonly GridManager _gridManager;

        public BoardMutator(PlayfieldBoard board, PlayfieldItemsFactory factory, GridManager gridManager)
        {
            _board = board;
            _factory = factory;
            _gridManager = gridManager;
        }

        public async UniTask DestroyCells(IEnumerable<Vector2Int> cells, IBoardContext context)
        {
            var powerUpsToActivate = new List<PlayfieldItem>();

            foreach (var cell in cells)
            {
                var item = _board.Get(cell);
                if (item == null) continue;
                _board.Clear(cell);

                if (item.IsPowerUp)
                    powerUpsToActivate.Add(item);
                else
                    item.DestroyItem();
            }

            await UniTask.WaitForSeconds(ProjectConstants.ITEM_DESTROY_ANIM_DURATION);

            foreach (var powerUp in powerUpsToActivate)
            {
                var ctx = new ActivationContext(powerUp.OccupiedCell);
                
                await powerUp.PowerUp.Activate(ctx, context);
                powerUp.DestroyItem();
            }
        }

        public PlayfieldItem SpawnRocketAt(Vector2Int cell, RocketOrientation orientation)
        {
            DestroyExistingAt(cell);
            var item = _factory.SpawnRocket(orientation, _gridManager.PlayfieldItemsContainer);
            item.SetVisibility(true);
            PlaceAt(cell, item);
            return item;
        }

        public PlayfieldItem SpawnBombAt(Vector2Int cell)
        {
            DestroyExistingAt(cell);
            var item = _factory.SpawnBomb(_gridManager.PlayfieldItemsContainer);
            item.SetVisibility(true);
            PlaceAt(cell, item);
            return item;
        }

        public PlayfieldItem SpawnPlaneAt(Vector2Int cell)
        {
            DestroyExistingAt(cell);
            var item = _factory.SpawnPlane(_gridManager.PlayfieldItemsContainer);
            item.SetVisibility(true);
            PlaceAt(cell, item);
            return item;
        }

        public PlayfieldItem SpawnDiscoAt(Vector2Int cell)
        {
            DestroyExistingAt(cell);
            var item = _factory.SpawnDisco(_gridManager.PlayfieldItemsContainer);
            item.SetVisibility(true);
            PlaceAt(cell, item);
            return item;
        }

        void DestroyExistingAt(Vector2Int cell)
        {
            var existing = _board.Get(cell);
            if (existing == null) return;
            _board.Clear(cell);
            existing.DestroyItem();
        }

        void PlaceAt(Vector2Int cell, PlayfieldItem item)
        {
            _board.Set(cell, item);
            item.OccupyCell(cell);
            var pos = _gridManager.GetPositionForCell(cell);
            item.MoveTo(pos, MoveAnimationType.None);
        }
    }
}