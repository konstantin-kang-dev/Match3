using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Game.Utils;
using R3;
using UnityEngine;

namespace Game
{
    public class BoardMutator
    {
        private readonly IBoard _board;
        private readonly PlayfieldItemsFactory _factory;
        private readonly GridManager _gridManager;
        private readonly IVfxService _vfxService;
        private readonly PowerUpAnimator _animator;
        
        public BoardMutator(
            IBoard board,
            PlayfieldItemsFactory factory,
            GridManager gridManager,
            IVfxService vfxService,
            PowerUpAnimator animator)
        {
            _board = board;
            _factory = factory;
            _gridManager = gridManager;
            _vfxService = vfxService;
            _animator = animator;
        }

        public async UniTask DestroyCells(
            IEnumerable<Vector2Int> cells,
            IBoardContext context,
            DestroyMode mode = DestroyMode.Animated,
            bool playVfx = true)
        {
            var powerUpsToActivate = new List<PlayfieldItem>();

            foreach (var cell in cells)
            {
                var item = _board.Get(cell);
                if (item == null) continue;
                _board.Clear(cell);

                if (item.IsPowerUp)
                {
                    powerUpsToActivate.Add(item);
                }
                else
                {
                    if (playVfx)
                    {
                        var capturedCell = cell;
                        item.OnDestroyed.Subscribe(_ =>
                        {
                            _vfxService.PlayAtCell(PlayfieldVfxType.MatchDestroy, capturedCell);
                        });
                    }

                    item.DestroyItem(mode);
                }
            }

            if (mode == DestroyMode.Animated)
                await UniTask.WaitForSeconds(ProjectConstants.ITEM_DESTROY_ANIM_DURATION);

            foreach (var powerUp in powerUpsToActivate)
            {
                var ctx = new ActivationContext(powerUp.OccupiedCell, powerUp);
                await powerUp.PowerUp.Activate(ctx, context);
                powerUp.DestroyItem(DestroyMode.Instant);
            }
        }

        public PlayfieldItem SpawnRocketAt(Vector2Int cell, RocketOrientation orientation)
        {
            DestroyExistingAt(cell);
            var item = _factory.SpawnRocket(orientation, _gridManager.PlayfieldItemsContainer);
            _animator.PlayRocketSpawn(item);
            _vfxService.PlayAtCell(PlayfieldVfxType.PowerUpSpawn, cell);
            PlaceAt(cell, item);
            return item;
        }

        public PlayfieldItem SpawnBombAt(Vector2Int cell)
        {
            DestroyExistingAt(cell);
            var item = _factory.SpawnBomb(_gridManager.PlayfieldItemsContainer);
            _animator.PlayBombSpawn(item);
            _vfxService.PlayAtCell(PlayfieldVfxType.PowerUpSpawn, cell);
            PlaceAt(cell, item);
            return item;
        }

        public PlayfieldItem SpawnPlaneAt(Vector2Int cell)
        {
            DestroyExistingAt(cell);
            var item = _factory.SpawnPlane(_gridManager.PlayfieldItemsContainer);
            _animator.PlayPlaneSpawn(item);
            _vfxService.PlayAtCell(PlayfieldVfxType.PowerUpSpawn, cell);
            PlaceAt(cell, item);
            return item;
        }

        public PlayfieldItem SpawnDiscoAt(Vector2Int cell)
        {
            DestroyExistingAt(cell);
            var item = _factory.SpawnDisco(_gridManager.PlayfieldItemsContainer);
            _animator.PlayDiscoSpawn(item);
            _vfxService.PlayAtCell(PlayfieldVfxType.PowerUpSpawn, cell);
            PlaceAt(cell, item);
            return item;
        }

        void DestroyExistingAt(Vector2Int cell)
        {
            var existing = _board.Get(cell);
            if (existing == null) return;
            _board.Clear(cell);
            existing.DestroyItem(DestroyMode.Instant);
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