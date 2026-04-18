using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class PlayfieldManager
    {
        PlayfieldItemPresenter[,] _playfieldItems;

        readonly GridManager _gridManager;
        readonly PlayfieldItemsFactory _playfieldItemsFactory;

        public PlayfieldManager(GridManager gridManager, PlayfieldItemsFactory factory)
        {
            _gridManager = gridManager;
            _playfieldItemsFactory = factory;
        }

        public void Init()
        {
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
                    PlayfieldItemPresenter playfieldItemPresenter = _playfieldItemsFactory.SpawnItem(PlayfieldItemType.CommonRed, _gridManager.transform);
                    _playfieldItems[x, y] = playfieldItemPresenter;
                    Vector2 targetPos = _gridManager.GetPositionForCell(new Vector2Int(x, y));
                    Vector2 startPos = targetPos + new Vector2(0, 3000);

                    playfieldItemPresenter.Visuals.MoveTo(startPos, true);
                    playfieldItemPresenter.Visuals.MoveTo(targetPos);

                }

                await UniTask.WaitForSeconds(0.1f);
            }

        }

        public void TrySwap(Vector2Int from, Vector2Int to)
        {

        }

        public void MoveTo(Vector2Int from, Vector2Int to)
        {
            PlayfieldItemPresenter presenter = _playfieldItems[from.x, from.y];
            Vector2 targetPos = _gridManager.GetPositionForCell(to);

            presenter.Visuals.MoveTo(targetPos);
            _playfieldItems[from.x, from.y] = null;
            _playfieldItems[to.x, to.y] = presenter;
        }

        private List<Vector2Int> FindMatches()
        {
            HashSet<Vector2Int> matched = new HashSet<Vector2Int>();

            Vector2Int gridSize = _gridManager.GridSize;

            // Горизонталь
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x - 2; x++)
                {
                    var a = _playfieldItems[x, y];
                    var b = _playfieldItems[x + 1, y];
                    var c = _playfieldItems[x + 2, y];

                    if (a.Model.Type == b.Model.Type && b.Model.Type == c.Model.Type)
                    {
                        matched.Add(new Vector2Int(x, y));
                        matched.Add(new Vector2Int(x + 1, y));
                        matched.Add(new Vector2Int(x + 2, y));
                    }
                }
            }

            // Вертикаль
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y - 2; y++)
                {
                    var a = _playfieldItems[x, y];
                    var b = _playfieldItems[x, y + 1];
                    var c = _playfieldItems[x, y + 2];

                    if (a.Model.Type == b.Model.Type && b.Model.Type == c.Model.Type)
                    {
                        matched.Add(new Vector2Int(x, y));
                        matched.Add(new Vector2Int(x, y + 1));
                        matched.Add(new Vector2Int(x, y + 2));
                    }
                }
            }

            return new List<Vector2Int>(matched);
        }
    }
}
