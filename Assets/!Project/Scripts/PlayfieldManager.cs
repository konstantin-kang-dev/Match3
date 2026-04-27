using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Game.Utils;

namespace Game
{
    public class PlayfieldManager : ISwapRequester
{
    readonly GridManager _gridManager;
    readonly PlayfieldItemsFactory _factory;

    PlayfieldBoard _board;
    MatchDetector _matchDetector;
    MatchResolver _matchResolver;
    BoardMutator _boardMutator;
    BoardContext _boardContext;
    BoardCollapser _collapser;
    BoardFiller _filler;
    PlayfieldAnimator _playfieldAnimator;

    public Observable<MatchResolvedEvent> OnMatchResolved => _matchResolver.OnMatchResolved;

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
        _boardMutator = new BoardMutator(_board, _factory, _gridManager);
        _boardContext = new BoardContext(_board, _boardMutator, _gridManager);
        _matchResolver = new MatchResolver(_boardMutator, _gridManager, _boardContext);
        _collapser = new BoardCollapser(_board);
        _filler = new BoardFiller(_board, _gridManager, _factory);
        _playfieldAnimator = new PlayfieldAnimator(_gridManager);

        var refillMovements = _filler.Refill();
        await _playfieldAnimator.AnimateFall(refillMovements);
        IsMatching = false;
    }

    public void TrySwap(Vector2Int from, Vector2Int direction)
    {
        if (IsMatching) return;
        Vector2Int to = from + direction;
        if (!_gridManager.IsValidCell(to)) return;

        var itemA = _board.Get(from);
        var itemB = _board.Get(to);

        bool aIsPowerUp = itemA != null && itemA.IsPowerUp;
        bool bIsPowerUp = itemB != null && itemB.IsPowerUp;

        if (aIsPowerUp && bIsPowerUp)
        {
            // комбо — пока не реализовано
            return;
        }

        if (aIsPowerUp || bIsPowerUp)
        {
            HandlePowerUpSwap(from, to, aIsPowerUp ? itemA : itemB).Forget();
            return;
        }

        SwapItems(from, to);
        HandleSwapProcessed(from, to).Forget();
    }

    void SwapItems(Vector2Int from, Vector2Int to)
    {
        _lastSwapFrom = from;
        _lastSwapTo = to;
        var itemA = _board.Get(from);
        var itemB = _board.Get(to);
        var movementA = new CellMovement(itemA, from, to, false);
        var movementB = new CellMovement(itemB, to, from, false);
        itemA.OccupyCell(to);
        itemB.OccupyCell(from);
        _playfieldAnimator.MoveItems(new List<CellMovement> { movementA, movementB });
        _board.Swap(from, to);
    }

    void RevertSwap() => SwapItems(_lastSwapTo, _lastSwapFrom);

    async UniTask HandleSwapProcessed(Vector2Int from, Vector2Int to)
    {
        IsMatching = true;
        await UniTask.WaitForSeconds(0.15f);

        try
        {
            var groups = _matchDetector.FindMatches(new[] { from, to });

            if (groups.Count == 0)
            {
                RevertSwap();
                await UniTask.WaitForSeconds(0.15f);
                return;
            }

            await _matchResolver.Resolve(groups, swapCell: to, cascade: 0);

            await ProcessCascadeAfterMutation();
        }
        finally
        {
            IsMatching = false;
        }
    }
    async UniTask HandlePowerUpSwap(Vector2Int from, Vector2Int to, PlayfieldItem powerUp)
    {
        IsMatching = true;
        try
        {
            // Определяем цвет цели до свапа
            var targetItem = _board.Get(from) == powerUp ? _board.Get(to) : _board.Get(from);
            var swappedColor = targetItem?.Color;

            SwapItems(from, to);
            await UniTask.WaitForSeconds(0.15f);

            Vector2Int activationCell = powerUp.OccupiedCell;
            _board.Clear(activationCell);

            var activationContext = new ActivationContext(activationCell, swappedColor);
            await powerUp.PowerUp.Activate(activationContext, _boardContext);

            powerUp.DestroyItem();

            await ProcessCascadeAfterMutation();
        }
        finally
        {
            IsMatching = false;
        }
    }
    async UniTask ProcessCascadeAfterMutation()
    {
        int cascade = 0;
        while (true)
        {
            var collapseMovements = _collapser.Collapse();
            var refillMovements = _filler.Refill();
            var allMovements = new List<CellMovement>(collapseMovements.Count + refillMovements.Count);
            allMovements.AddRange(collapseMovements);
            allMovements.AddRange(refillMovements);
            await _playfieldAnimator.AnimateFall(allMovements);
            await UniTask.WaitForSeconds(0.15f);

            var groups = _matchDetector.FindMatches(null);
            if (groups.Count == 0) break;

            await _matchResolver.Resolve(groups, swapCell: null, cascade);
            cascade++;
        }
    }
    }

}