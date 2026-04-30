using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using DG.Tweening;
using Game.Utils;
using R3;
using UnityEngine;

namespace Game
{
    public class BoardMutator
    {
        private readonly BoardState _board;
        private readonly PlayfieldItemsFactory _factory;
        private readonly GridManager _gridManager;
        private readonly IVfxService _vfxService;
        private readonly PowerUpAnimator _animator;
        private readonly BoardActivityTracker _tracker;

        public BoardMutator(
            BoardState board,
            PlayfieldItemsFactory factory,
            GridManager gridManager,
            IVfxService vfxService,
            PowerUpAnimator animator,
            BoardActivityTracker tracker)
        {
            _board = board;
            _factory = factory;
            _gridManager = gridManager;
            _vfxService = vfxService;
            _animator = animator;
            _tracker = tracker;
        }

        public async UniTask DestroyCells(
            IEnumerable<Vector2Int> cells,
            IBoardContext context,
            DestroyMode mode = DestroyMode.Animated,
            bool playVfx = true)
        {
            using (_tracker.BeginActivity())
            {
                var cellsList = new List<Vector2Int>();
                foreach (var c in cells) cellsList.Add(c);

                foreach (var cell in cellsList)
                {
                    var s = _board.Get(cell);
                    var itemHash = s.Item == null ? "null" : s.Item.GetHashCode().ToString();
                    var disposed = s.Item != null && s.Item.IsDisposed;
                }

                var powerUpsToActivate = new List<(PlayfieldItem item, CellSlot slot)>();
                var slotsToFinalize = new List<CellSlot>();

                foreach (var cell in cellsList)
                {
                    var slot = _board.Get(cell);

                    if (slot.State == CellState.Destroying || slot.State == CellState.Empty) continue;

                    var item = slot.Item;
                    if (item == null) continue;
                    
                    if (item.IsActivating) continue;
                    
                    if (slot.State == CellState.Falling)
                        item.RectTransform.DOKill();

                    slot.SetDestroying();

                    if (item.IsPowerUp)
                    {
                        powerUpsToActivate.Add((item, slot));
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
                        slotsToFinalize.Add(slot);
                    }
                }

                if (mode == DestroyMode.Animated)
                    await UniTask.WaitForSeconds(ProjectConstants.ITEM_DESTROY_ANIM_DURATION);

                foreach (var slot in slotsToFinalize)
                    slot.SetEmpty();

                if (powerUpsToActivate.Count > 0)
                {
                    var activationTasks = new List<UniTask>(powerUpsToActivate.Count);
                    foreach (var (powerUp, slot) in powerUpsToActivate)
                        activationTasks.Add(ActivatePowerUp(powerUp, slot, context));
    
                    await UniTask.WhenAll(activationTasks);
                }
            }
        }
        
        async UniTask ActivatePowerUp(PlayfieldItem powerUpItem, CellSlot slot, IBoardContext context)
        {
            powerUpItem.SetActivating(true);
            _tracker.Freeze();
            try
            {
                var ctx = new ActivationContext(powerUpItem.OccupiedCell, powerUpItem);
                await powerUpItem.PowerUp.Activate(ctx, context);

                if (!powerUpItem.PowerUp.SelfDestroys)
                {
                    powerUpItem.DestroyItem(DestroyMode.Instant);
                    slot.SetEmpty();
                }
                else
                {
                    slot.SetEmpty();
                }
            }
            finally
            {
                powerUpItem.SetActivating(false);
                _tracker.Unfreeze();
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
            _animator.PlayBalloonSpawn(item);
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
            var slot = _board.Get(cell);
            if (slot.Item == null) return;

            var existing = slot.Item;
            slot.SetDestroying();
            existing.DestroyItem(DestroyMode.Instant);
            slot.ClearItem();
        }

        void PlaceAt(Vector2Int cell, PlayfieldItem item)
        {
            var slot = _board.Get(cell);
            item.OccupyCell(cell);
            var pos = _gridManager.GetPositionForCell(cell);
            item.MoveTo(pos, MoveAnimationType.None);
            slot.SetOccupied(item);
        }
    }
}