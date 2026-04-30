using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Game
{
    public class PlayfieldManager : ISwapRequester
    {
        readonly GridManager _gridManager;
        readonly BoardState _board;
        readonly MatchDetector _matchDetector;
        readonly MatchResolver _matchResolver;
        readonly BoardMutator _boardMutator;
        readonly IBoardContext _boardContext;
        readonly BoardActivityTracker _tracker;

        public Observable<MatchResolvedEvent> OnMatchResolved => _matchResolver.OnMatchResolved;

        Vector2Int _lastSwapFrom;
        Vector2Int _lastSwapTo;

#if UNITY_EDITOR
        public BoardMutator BoardMutator => _boardMutator;
        public IBoardContext BoardContext => _boardContext;
#endif

        public PlayfieldManager(
            GridManager gridManager,
            BoardState board,
            MatchDetector matchDetector,
            BoardMutator boardMutator,
            IBoardContext boardContext,
            MatchResolver matchResolver,
            BoardActivityTracker tracker)
        {
            _gridManager = gridManager;
            _board = board;
            _matchDetector = matchDetector;
            _boardMutator = boardMutator;
            _boardContext = boardContext;
            _matchResolver = matchResolver;
            _tracker = tracker;
        }

        public async UniTask Init()
        {
            
            
            
            
            

            
            await WaitUntilIdle();
        }

        public async void TrySwap(Vector2Int from, Vector2Int direction)
        {
            
            if (!_tracker.IsIdle) return;

            using (_tracker.BeginActivity())
            {
                Vector2Int to = from + direction;
                if (!_gridManager.IsValidCell(to)) return;

                var slotFrom = _board.Get(from);
                var slotTo = _board.Get(to);

                
                if (slotFrom.State != CellState.Occupied || slotTo.State != CellState.Occupied) return;

                var itemA = slotFrom.Item;
                var itemB = slotTo.Item;

                bool aIsPowerUp = itemA != null && itemA.IsPowerUp;
                bool bIsPowerUp = itemB != null && itemB.IsPowerUp;

                if (aIsPowerUp && bIsPowerUp)
                {
                    
                    return;
                }

                if (aIsPowerUp || bIsPowerUp)
                {
                    await HandlePowerUpSwap(from, to, aIsPowerUp ? itemA : itemB);
                    return;
                }

                await HandleColoredSwap(from, to);
            }

        }

        async UniTask HandleColoredSwap(Vector2Int from, Vector2Int to)
        {
            SwapItems(from, to);
            await UniTask.WaitForSeconds(0.15f);

            var groups = _matchDetector.FindMatches(new[] { from, to });

            if (groups.Count == 0)
            {
                // Откат — ходов нет.
                RevertSwap();
                await UniTask.WaitForSeconds(0.15f);
                return;
            }

            // Резолв запускаем явно — Scanner не слушает свапы.
            // Дальнейшие каскады возьмёт на себя реактивный пайплайн через MatchScanner.
            await _matchResolver.Resolve(groups, swapCell: to, cascade: 0);
        }

        async UniTask HandlePowerUpSwap(Vector2Int from, Vector2Int to, PlayfieldItem powerUpItem)
        {
            var slotFrom = _board.Get(from);
            var targetItem = slotFrom.Item == powerUpItem ? _board.Get(to).Item : slotFrom.Item;
            var swappedColor = targetItem?.Color;

            if(powerUpItem.IsActivating) return;
            SwapItems(from, to);
            await UniTask.WaitForSeconds(0.15f);

            Vector2Int activationCell = powerUpItem.OccupiedCell;
            using (_tracker.BeginActivity())
            {
                powerUpItem.SetActivating(true);
                _tracker.Freeze();
                try
                {

                    var ctx = new ActivationContext(activationCell, powerUpItem, swappedColor);
                    await powerUpItem.PowerUp.Activate(ctx, _boardContext);
                    CellSlot slot = _board.Get(activationCell);
                    
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

        }

        void SwapItems(Vector2Int from, Vector2Int to)
        {
            _lastSwapFrom = from;
            _lastSwapTo = to;

            var slotFrom = _board.Get(from);
            var slotTo = _board.Get(to);
            var itemA = slotFrom.Item;
            var itemB = slotTo.Item;

            // Логически: меняем Item'ы между слотами без событий.
            _board.Swap(from, to);

            // Обновляем Item.OccupiedCell — это координата фишки на доске.
            itemA.OccupyCell(to);
            itemB.OccupyCell(from);

            // Визуальная анимация — фишки физически летят в новые позиции.
            itemA.MoveTo(_gridManager.GetPositionForCell(to), MoveAnimationType.Move);
            itemB.MoveTo(_gridManager.GetPositionForCell(from), MoveAnimationType.Move);
        }

        void RevertSwap() => SwapItems(_lastSwapTo, _lastSwapFrom);

        UniTask WaitUntilIdle()
        {
            if (_tracker.IsIdle) return UniTask.CompletedTask;
            return _tracker.OnIdleChanged
                .Where(idle => idle)
                .FirstAsync()
                .AsUniTask();
        }
    }
}