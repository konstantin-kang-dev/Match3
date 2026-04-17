using Game;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class PlayfieldManager
    {
        PlayfieldItemPresenter[,] _playfieldItems;

        readonly GridManager _gridManager;

        public PlayfieldManager(GridManager gridManager)
        {
            _gridManager = gridManager;
        }

        public void Init()
        {
            Vector2 gridSize = _gridManager.GridSize;
            _playfieldItems = new PlayfieldItemPresenter[(int)gridSize.x, (int)gridSize.y];

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    SpawnItem(new Vector2Int(x, y));
                }
            }
        }

        void SpawnItem(Vector2Int targetCell)
        {/*
            Vector2 cellSize = _gridManager.CellSize;
            PlayfieldItemPresenter item = Instantiate(ItemPrefab);
            RectTransform rectTransform = item.GetComponent<RectTransform>();
            rectTransform.SetParent(transform);
            rectTransform.anchoredPosition = targetCell * cellSize + (cellSize / 2f);

            _playfieldItems[targetCell.x, targetCell.y] = item;
            */
        }

        public void TrySwap(Vector2Int from, Vector2Int to)
        {

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
