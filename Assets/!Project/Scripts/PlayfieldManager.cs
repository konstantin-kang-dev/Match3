using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Game
{


    public class PlayfieldManager
    {
        readonly GridManager _gridManager;
        readonly PlayfieldItemsFactory _factory;

        PlayfieldBoard _board;
        MatchDetector _matchDetector;
        BoardCollapser _collapser;
        BoardFiller _filler;
        PlayfieldAnimator _playfieldAnimator;

        readonly Subject<MatchResolvedEvent> _onMatchResolved = new();
        public Observable<MatchResolvedEvent> OnMatchResolved => _onMatchResolved.AsObservable();

        public bool IsMatching { get; private set; }

        Vector2Int _lastSwapFrom;
        Vector2Int _lastSwapTo;

        public PlayfieldManager(GridManager gridManager, PlayfieldItemsFactory factory)
        {
            _gridManager = gridManager;
            _factory = factory;
        }

        public async UniTask Init()
        {
            IsMatching = true;
            _board = new PlayfieldBoard(_gridManager.GridSize);
            _matchDetector = new MatchDetector(_board);
            _collapser = new BoardCollapser(_board);
            _filler = new BoardFiller(_board, _gridManager, _factory);
            _playfieldAnimator = new PlayfieldAnimator(_gridManager);

            var refillMovements = _filler.Refill();
            await _playfieldAnimator.AnimateFall(refillMovements);
            IsMatching = false;
            Debug.Log("[PlayfieldManager] Initialized.");
        }

        public void TrySwap(Vector2Int from, Vector2Int direction)
        {
            if (IsMatching) return;

            Vector2Int to = from + direction;
            if (!_gridManager.IsValidCell(to)) return;

            SwapItems(from, to);
            HandleSwapProcessed(from, to).Forget();
        }

        void SwapItems(Vector2Int from, Vector2Int to)
        {
            _lastSwapFrom = from;
            _lastSwapTo = to;

            var itemA = _board.Get(from);
            var itemB = _board.Get(to);
            CellMovement movementDataA = new CellMovement(itemA, from, to, false);
            CellMovement movementDataB = new CellMovement(itemB, to, from, false);

            itemA.OccupyCell(to);
            itemB.OccupyCell(from);

            _playfieldAnimator.MoveItems(new List<CellMovement>() { movementDataA, movementDataB});

            _board.Swap(from, to);
        }

        void RevertSwap() => SwapItems(_lastSwapTo, _lastSwapFrom);

        async UniTask HandleSwapProcessed(Vector2Int from, Vector2Int to)
        {
            IsMatching = true;

            await UniTask.WaitForSeconds(0.15f);
            try
            {
                IEnumerable<Vector2Int> cellsToCheck = new[] { from, to };
                var groups = _matchDetector.FindMatches(cellsToCheck);

                if (groups.Count == 0)
                {
                    RevertSwap();
                    await UniTask.WaitForSeconds(0.15f);
                    return;
                }

                int cascade = 0;
                while (groups.Count > 0)
                {

                    EmitMatchEvents(groups, cascade);

                    await DestroyMatches(groups);

                    var collapseMovements = _collapser.Collapse();
                    var refillMovements = _filler.Refill();

                    var allMovements = new List<CellMovement>(collapseMovements.Count + refillMovements.Count);
                    allMovements.AddRange(collapseMovements);
                    allMovements.AddRange(refillMovements);

                    await _playfieldAnimator.AnimateFall(allMovements);

                    await UniTask.WaitForSeconds(0.15f);
                    groups = _matchDetector.FindMatches(null);
                    cascade++;
                }
            }
            finally
            {
                IsMatching = false;
            }
        }

        void EmitMatchEvents(List<MatchGroup> groups, int cascade)
        {
            foreach (var group in groups)
            {
                _onMatchResolved.OnNext(new MatchResolvedEvent(
                    cells: group.Cells,
                    type: group.Type,
                    shape: group.Shape,
                    cascadeLevel: cascade,
                    worldCenter: ComputeCenter(group.Cells)
                ));
            }
        }

        async UniTask DestroyMatches(List<MatchGroup> groups)
        {
            foreach (var group in groups)
                foreach (var cell in group.Cells)
                {
                    var item = _board.Get(cell);
                    if (item == null) continue;
                    _board.Clear(cell);
                    item.DestroyItem();
                }

            await UniTask.WaitForSeconds(ProjectConstants.ITEM_DESTROY_ANIM_DURATION);
        }

        Vector2 ComputeCenter(IReadOnlyList<Vector2Int> cells)
        {
            Vector2 sum = Vector2.zero;
            foreach (var c in cells)
                sum += _gridManager.GetPositionForCell(c);
            return sum / cells.Count;
        }
    }
}