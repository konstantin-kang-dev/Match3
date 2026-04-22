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

        public void Init()
        {
            _board = new PlayfieldBoard(_gridManager.GridSize);
            _matchDetector = new MatchDetector(_board);
            _collapser = new BoardCollapser(_board);
            _filler = new BoardFiller(_board, _gridManager, _factory);

            _filler.SpawnInitial().Forget();
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

            itemA.OccupyCell(to, true);
            itemB.OccupyCell(from, true);

            _board.Swap(from, to);
        }

        void RevertSwap() => SwapItems(_lastSwapTo, _lastSwapFrom);

        async UniTask HandleSwapProcessed(Vector2Int from, Vector2Int to)
        {
            IsMatching = true;
            try
            {
                IEnumerable<Vector2Int> cellsToCheck = new[] { from, to };
                var groups = _matchDetector.FindMatches(cellsToCheck);

                if (groups.Count == 0)
                {
                    await UniTask.WaitForSeconds(0.3f);
                    RevertSwap();
                    await UniTask.WaitForSeconds(0.3f);
                    return;
                }

                int cascade = 0;
                while (groups.Count > 0)
                {
                    EmitMatchEvents(groups, cascade);

                    await UniTask.WaitForSeconds(0.3f);
                    DestroyMatches(groups);
                    await UniTask.WaitForSeconds(0.3f);

                    await _collapser.Collapse();
                    await _filler.Refill();
                    await UniTask.WaitForSeconds(0.3f);

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

        void DestroyMatches(List<MatchGroup> groups)
        {
            foreach (var group in groups)
                foreach (var cell in group.Cells)
                {
                    var item = _board.Get(cell);
                    if (item == null) continue;
                    _board.Clear(cell);
                    item.DestroyItem();
                }
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