using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Game
{
    public partial class PlayfieldManager
    {
        PlayfieldItemPresenter[,] _playfieldItems;

        readonly GridManager _gridManager;
        readonly PlayfieldItemsFactory _playfieldItemsFactory;

        Subject<List<Vector2Int>> OnSwapProcessed = new Subject<List<Vector2Int>>();

        public bool IsMatching { get; private set; } = false;

        Vector2Int _lastSwapFrom;
        Vector2Int _lastSwapTo;

        public PlayfieldManager(GridManager gridManager, PlayfieldItemsFactory factory)
        {
            _gridManager = gridManager;
            _playfieldItemsFactory = factory;
        }

        public void Init()
        {
            OnSwapProcessed.Subscribe(HandleSwapProcessed);
            SpawnItems();
            Debug.Log($"[PlayfieldManager] Initialized.");
        }

        async void SpawnItems()
        {
            Vector2Int gridSize = _gridManager.GridSize;
            _playfieldItems = new PlayfieldItemPresenter[gridSize.x, gridSize.y];

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector2Int cell = new Vector2Int(x, y);

                    PlayfieldItemType randomType = GetTypeWithoutMatch(x, y);
                    PlayfieldItemPresenter playfieldItemPresenter = _playfieldItemsFactory.SpawnItem(randomType, _gridManager.transform);
                    _playfieldItems[cell.x, cell.y] = playfieldItemPresenter;

                    Vector2 targetPos = _gridManager.GetPositionForCell(new Vector2Int(x, y));
                    Vector2 startPos = targetPos + new Vector2(0, 3000);

                    playfieldItemPresenter.OccupyCell(cell);
                    playfieldItemPresenter.View.MoveTo(startPos, doInstantly: true);
                    playfieldItemPresenter.View.MoveTo(targetPos);
                }

                await UniTask.WaitForSeconds(0.1f);
            }
        }

        public void TrySwap(Vector2Int from, Vector2Int direction)
        {
            if (IsMatching) return;

            Vector2Int to = from + direction;

            if (!_gridManager.IsValidCell(to))
            {
                return;
            }

            SwapItems(from, to);

            OnSwapProcessed.OnNext(new List<Vector2Int> { from, to });
        }

        void SwapItems(Vector2Int from, Vector2Int to)
        {
            _lastSwapFrom = from;
            _lastSwapTo = to;

            PlayfieldItemPresenter itemA = _playfieldItems[from.x, from.y];
            PlayfieldItemPresenter itemB = _playfieldItems[to.x, to.y];

            itemA.OccupyCell(to, true);
            itemB.OccupyCell(from, true);

            _playfieldItems[from.x, from.y] = itemB;
            _playfieldItems[to.x, to.y] = itemA;
        }
        void RevertSwap() => SwapItems(_lastSwapFrom, _lastSwapTo);

        async void HandleSwapProcessed(List<Vector2Int> swappedCells)
        {
            IsMatching = true;

            //Debug.Log($"[PlayfieldManager] Swap processed, checking for matches...");
            try
            {
                IEnumerable<Vector2Int> cellsToCheck = swappedCells;
                List<Vector2Int> matches = FindMatches(cellsToCheck);

                if (matches.Count == 0)
                {
                    await UniTask.WaitForSeconds(0.3f);
                    RevertSwap();

                    await UniTask.WaitForSeconds(0.3f);
                    IsMatching = false;
                    return;
                }

                while (true)
                {
                    matches = FindMatches(cellsToCheck);

                    if (matches.Count == 0) break;

                    await UniTask.WaitForSeconds(0.3f);
                    ProcessMatch(matches);
                    await UniTask.WaitForSeconds(0.3f);

                    CollapseColumns();

                    await UniTask.WaitForSeconds(0.3f);
                    await RefillColumns();
                    await UniTask.WaitForSeconds(0.3f);

                    cellsToCheck = null;
                }
            }
            finally
            {
                IsMatching = false;
                //Debug.Log($"[PlayfieldManager] Matches check completed.");
            }

        }

        void ProcessMatch(List<Vector2Int> matches)
        {
            foreach (var cell in matches)
            {
                PlayfieldItemPresenter item = _playfieldItems[cell.x, cell.y];
                _playfieldItems[cell.x, cell.y] = null;
                item.DestroyItem();
            }
        }

        void CollapseColumns()
        {
            Vector2Int gridSize = _gridManager.GridSize;

            for (int x = 0; x < gridSize.x; x++)
            {
                int writeY = 0;

                for (int readY = 0; readY < gridSize.y; readY++)
                {
                    if (_playfieldItems[x, readY] == null) continue;

                    if (readY != writeY)
                    {
                        Vector2Int to = new Vector2Int(x, writeY);
                        _playfieldItems[x, writeY] = _playfieldItems[x, readY];
                        _playfieldItems[x, readY] = null;
                        _playfieldItems[x, writeY].OccupyCell(to, animate: true);
                    }

                    writeY++;
                }
            }
        }

        async UniTask RefillColumns()
        {
            Vector2Int gridSize = _gridManager.GridSize;

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (_playfieldItems[x, y] != null) continue;

                    PlayfieldItemType type = GetTypeWithoutMatch(x, y);
                    PlayfieldItemPresenter item = _playfieldItemsFactory.SpawnItem(type, _gridManager.transform);
                    _playfieldItems[x, y] = item;

                    Vector2 targetPos = _gridManager.GetPositionForCell(new Vector2Int(x, y));
                    Vector2 startPos = targetPos + new Vector2(0, 3000);

                    item.OccupyCell(new Vector2Int(x, y));
                    item.View.MoveTo(startPos, doInstantly: true);
                    item.View.MoveTo(targetPos);
                }

                await UniTask.WaitForSeconds(0.05f);
            }
        }

        private List<Vector2Int> FindMatches(IEnumerable<Vector2Int> changedCells)
        {
            HashSet<Vector2Int> matched = new HashSet<Vector2Int>();

            if (changedCells == null)
            {
                var grid = _gridManager.GridSize;
                for (int x = 0; x < grid.x; x++)
                    for (int y = 0; y < grid.y; y++)
                    {
                        var pos = new Vector2Int(x, y);
                        CheckLine(pos, Vector2Int.right, matched);
                        CheckLine(pos, Vector2Int.up, matched);
                    }
            }
            else
            {
                HashSet<Vector2Int> toCheck = BuildCheckSet(changedCells);
                foreach (var pos in toCheck)
                {
                    CheckLine(pos, Vector2Int.right, matched);
                    CheckLine(pos, Vector2Int.up, matched);
                }
            }

            return new List<Vector2Int>(matched);
        }

        private HashSet<Vector2Int> BuildCheckSet(IEnumerable<Vector2Int> changedCells)
        {
            var set = new HashSet<Vector2Int>();
            var grid = _gridManager.GridSize;

            foreach (var cell in changedCells)
            {
                for (int x = 0; x < grid.x; x++)
                    TryAdd(set, new Vector2Int(x, cell.y));
                for (int y = 0; y < grid.y; y++)
                    TryAdd(set, new Vector2Int(cell.x, y));
            }

            return set;
        }

        private void CheckLine(Vector2Int pos, Vector2Int dir, HashSet<Vector2Int> matched)
        {
            var grid = _gridManager.GridSize;

            Vector2Int prev = pos - dir;
            if (IsInBounds(prev, grid))
            {
                var prevType = GetItemType(prev);
                var curType = GetItemType(pos);
                if (prevType.HasValue && curType.HasValue && prevType == curType) return;
            }

            var run = new List<Vector2Int>();
            Vector2Int current = pos;

            while (IsInBounds(current, grid))
            {
                var type = GetItemType(current);
                if (!type.HasValue) break;
                if (run.Count > 0 && GetItemType(run[0]) != type) break;

                run.Add(current);
                current += dir;
            }

            if (run.Count >= 3)
                foreach (var cell in run)
                    matched.Add(cell);
        }

        private PlayfieldItemType? GetItemType(Vector2Int pos)
        {
            var item = _playfieldItems[pos.x, pos.y];
            return item?.Model.Type;
        }

        private bool IsInBounds(Vector2Int pos, Vector2Int grid) =>
            pos.x >= 0 && pos.x < grid.x && pos.y >= 0 && pos.y < grid.y;

        private void TryAdd(HashSet<Vector2Int> set, Vector2Int pos)
        {
            if (IsInBounds(pos, _gridManager.GridSize))
                set.Add(pos);
        }

        PlayfieldItemType GetTypeWithoutMatch(int x, int y)
        {
            var forbidden = new HashSet<PlayfieldItemType>();

            if (x >= 2)
            {
                var left1 = _playfieldItems[x - 1, y];
                var left2 = _playfieldItems[x - 2, y];
                if (left1 != null && left2 != null && left1.Model.Type == left2.Model.Type)
                    forbidden.Add(left1.Model.Type);
            }

            if (y >= 2)
            {
                var down1 = _playfieldItems[x, y - 1];
                var down2 = _playfieldItems[x, y - 2];
                if (down1 != null && down2 != null && down1.Model.Type == down2.Model.Type)
                    forbidden.Add(down1.Model.Type);
            }

            return ProjectUtils.GetRandomPlayfieldItemTypeExcluding(forbidden);
        }

    }
}