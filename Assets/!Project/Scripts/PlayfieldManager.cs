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
    private readonly GridManager _gridManager;
    private readonly PlayfieldItemsFactory _factory;
    private readonly IVfxService _vfxService;
    private readonly PowerUpAnimator _powerUpAnimator;
    
    private readonly IBoard _board;
    private readonly MatchDetector _matchDetector;
    private readonly MatchResolver _matchResolver;
    private readonly BoardMutator _boardMutator;

    private readonly IBoardContext _boardContext;
    private readonly BoardCollapser _collapser;
    private readonly BoardFiller _filler;
    private readonly PlayfieldAnimator _playfieldAnimator;
    
    public Observable<MatchResolvedEvent> OnMatchResolved => _matchResolver.OnMatchResolved;

    private bool IsMatching = false;

    private Vector2Int _lastSwapFrom;
    private Vector2Int _lastSwapTo;

    //DEBUG
#if UNITY_EDITOR
    public BoardMutator BoardMutator => _boardMutator;
    public IBoardContext BoardContext => _boardContext;
#endif
    
    public PlayfieldManager(
        GridManager gridManager,
        PlayfieldItemsFactory factory,
        IBoard board,
        MatchDetector matchDetector,
        BoardMutator boardMutator,
        IBoardContext boardContext,
        MatchResolver matchResolver,
        BoardCollapser collapser,
        BoardFiller filler,
        PlayfieldAnimator playfieldAnimator,
        IVfxService vfxService,
        PowerUpAnimator powerUpAnimator)
    {
        _gridManager = gridManager;
        _factory = factory;
        _powerUpAnimator = powerUpAnimator;
        _board = board;
        _matchDetector = matchDetector;
        _boardMutator = boardMutator;
        _boardContext = boardContext;
        _matchResolver = matchResolver;
        _collapser = collapser;
        _filler = filler;
        _playfieldAnimator = playfieldAnimator;
        _vfxService = vfxService;
    }

    public async UniTask Init()
    {
        IsMatching = true;
        
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
            var targetItem = _board.Get(from) == powerUp ? _board.Get(to) : _board.Get(from);
            var swappedColor = targetItem?.Color;

            SwapItems(from, to);
            await UniTask.WaitForSeconds(0.15f);

            Vector2Int activationCell = powerUp.OccupiedCell;
            _board.Clear(activationCell);

            var activationContext = new ActivationContext(activationCell, powerUp, swappedColor);
            await powerUp.PowerUp.Activate(activationContext, _boardContext);

            powerUp.DestroyItem(DestroyMode.Instant);

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