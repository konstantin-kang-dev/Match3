using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game;
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

                    PlayfieldItemType randomType = ProjectUtils.GetRandomPlayfieldItemType();
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
            Vector2Int to = from + direction;

            bool isValidCell = _gridManager.IsValidCell(to);

            
            if (isValidCell)
            {
                PlayfieldItemPresenter itemA = _playfieldItems[from.x, from.y];
                PlayfieldItemPresenter itemB = _playfieldItems[to.x, to.y];

                if (itemA == null || itemB == null) return;

                itemA.OccupyCell(to, true);
                itemB.OccupyCell(from, true);

                _playfieldItems[from.x, from.y] = itemB;
                _playfieldItems[to.x, to.y] = itemA;
            }

            List<Vector2Int> swappedCells = new List<Vector2Int>();
            swappedCells.Add(from);
            swappedCells.Add(to);

            OnSwapProcessed.OnNext(swappedCells);
        }

        async void HandleSwapProcessed(List<Vector2Int> swappedCells)
        {
            List<Vector2Int> matches = FindMatches(swappedCells);

            await UniTask.WaitForSeconds(0.3f);
            ProcessMatch(matches);
            await UniTask.WaitForSeconds(0.3f);

            CollapseColumns();
        }

        void ProcessMatch(List<Vector2Int> matches)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                Vector2Int cell = matches[i];
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
                        _playfieldItems[x, readY].OccupyCell(to, animate: true);
                    }

                    writeY++;
                }
            }
        }

        private List<Vector2Int> FindMatches(IEnumerable<Vector2Int> cells = null)
        {
            HashSet<Vector2Int> toCheck;

            if (cells == null)
            {
                toCheck = GetAllCells();
            }
            else
            {
                toCheck = new HashSet<Vector2Int>();
                foreach (var cell in cells)
                {
                    for (int dx = -2; dx <= 2; dx++)
                        TryAdd(toCheck, new Vector2Int(cell.x + dx, cell.y));
                    for (int dy = -2; dy <= 2; dy++)
                        TryAdd(toCheck, new Vector2Int(cell.x, cell.y + dy));
                }
            }

            HashSet<Vector2Int> matched = new HashSet<Vector2Int>();

            foreach (var pos in toCheck)
            {
                CheckHorizontal(pos, matched);
                CheckVertical(pos, matched);
            }

            return new List<Vector2Int>(matched);
        }

        private void CheckHorizontal(Vector2Int pos, HashSet<Vector2Int> matched)
        {
            var grid = _gridManager.GridSize;
            int x = pos.x, y = pos.y;
            if (x > grid.x - 3) return;

            var a = _playfieldItems[x, y];
            var b = _playfieldItems[x + 1, y];
            var c = _playfieldItems[x + 2, y];

            if (a == null || b == null || c == null) return;

            if (a.Model.Type == b.Model.Type && b.Model.Type == c.Model.Type)
            {
                matched.Add(new Vector2Int(x, y));
                matched.Add(new Vector2Int(x + 1, y));
                matched.Add(new Vector2Int(x + 2, y));
            }
        }

        private void CheckVertical(Vector2Int pos, HashSet<Vector2Int> matched)
        {
            var grid = _gridManager.GridSize;
            int x = pos.x, y = pos.y;
            if (y > grid.y - 3) return;

            var a = _playfieldItems[x, y];
            var b = _playfieldItems[x, y + 1];
            var c = _playfieldItems[x, y + 2];

            if (a == null || b == null || c == null) return;

            if (a.Model.Type == b.Model.Type && b.Model.Type == c.Model.Type)
            {
                matched.Add(new Vector2Int(x, y));
                matched.Add(new Vector2Int(x, y + 1));
                matched.Add(new Vector2Int(x, y + 2));
            }
        }

        private void TryAdd(HashSet<Vector2Int> set, Vector2Int pos)
        {
            var grid = _gridManager.GridSize;
            if (pos.x >= 0 && pos.x < grid.x && pos.y >= 0 && pos.y < grid.y)
                set.Add(pos);
        }

        private HashSet<Vector2Int> GetAllCells()
        {
            var grid = _gridManager.GridSize;
            var all = new HashSet<Vector2Int>();
            for (int y = 0; y < grid.y; y++)
                for (int x = 0; x < grid.x; x++)
                    all.Add(new Vector2Int(x, y));
            return all;
        }
    }
}
